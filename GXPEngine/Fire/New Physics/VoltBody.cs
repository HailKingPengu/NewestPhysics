/*
 *  VolatilePhysics - A 2D Physics Library for Networked Games
 *  Copyright (c) 2015-2016 - Alexander Shoulson - http://ashoulson.com
 *
 *  This software is provided 'as-is', without any express or implied
 *  warranty. In no event will the authors be held liable for any damages
 *  arising from the use of this software.
 *  Permission is granted to anyone to use this software for any purpose,
 *  including commercial applications, and to alter it and redistribute it
 *  freely, subject to the following restrictions:
 *  
 *  1. The origin of this software must not be misrepresented; you must not
 *     claim that you wrote the original software. If you use this software
 *     in a product, an acknowledgment in the product documentation would be
 *     appreciated but is not required.
 *  2. Altered source versions must be plainly marked as such, and must not be
 *     misrepresented as being the original software.
 *  3. This notice may not be removed or altered from any source distribution.
*/

using GXPEngine;
using GXPEngine.Managers;
using System;
using System.Runtime.InteropServices;

#if UNITY
using UnityEngine;
#endif

namespace Volatile
{
    public enum VoltBodyType
    {
        Static,
        Dynamic,
        Invalid,
    }

    public delegate bool VoltBodyFilter(VoltBody body);
    public delegate bool VoltCollisionFilter(VoltBody bodyA, VoltBody bodyB);
    
    public class VoltBody
      : GameObject, IVoltPoolable<VoltBody>
      , IIndexedValue
    {
        #region Interface
        IVoltPool<VoltBody> IVoltPoolable<VoltBody>.Pool { get; set; }
        void IVoltPoolable<VoltBody>.Reset() { this.Reset(); }
        int IIndexedValue.Index { get; set; }
        #endregion

        /// <summary>
        /// A predefined filter that disallows collisions between dynamic bodies.
        /// </summary>
        public static bool DisallowDynamic(VoltBody a, VoltBody b)
        {
            return
              (a != null) &&
              (b != null) &&
              (a.IsStatic || b.IsStatic);
        }

        #region History
        /// <summary>
        /// Tries to get a reference frame for a given number of ticks behind 
        /// the current tick. Returns true if a value was found, false if a
        /// value was not found (in which case we clamp to the nearest).
        /// </summary>
        public bool TryGetSpace(
          int ticksBehind,
          out Vec2 position,
          out Vec2 facing)
        {
            if (ticksBehind < 0)
                throw new ArgumentOutOfRangeException("ticksBehind");

            if (ticksBehind == 0)
            {
                position = this.Position;
                facing = this.Facing;
                return true;
            }

            if (this.history == null)
            {
                position = this.Position;
                facing = this.Facing;
                return false;
            }

            HistoryRecord record;
            bool found = this.history.TryGet(ticksBehind - 1, out record);
            position = record.position;
            facing = record.facing;
            return found;
        }

        /// <summary>
        /// Initializes the buffer for storing past body states/spaces.
        /// </summary>
        internal void AssignHistory(HistoryBuffer history)
        {
            VoltDebug.Assert(this.IsStatic == false);
            this.history = history;
        }

        /// <summary>
        /// Stores a snapshot of this body's current state/space to a tick.
        /// </summary>
        private void StoreState()
        {
            if (this.history != null)
                this.history.Store(this.currentState);
        }

        /// <summary>
        /// Retrieves a snapshot of the body's state/space at a tick.
        /// Logs an error and defaults to the current state if it can't be found.
        /// </summary>
        private HistoryRecord GetState(int ticksBehind)
        {
            if ((ticksBehind == 0) || (this.history == null))
                return this.currentState;

            HistoryRecord output;
            this.history.TryGet(ticksBehind - 1, out output);
            return output;
        }
        #endregion

        public static bool Filter(VoltBody body, VoltBodyFilter filter)
        {
            return ((filter == null) || (filter.Invoke(body) == true));
        }

        /// <summary>
        /// Static objects are considered to have infinite mass and cannot move.
        /// </summary>
        public bool IsStatic
        {
            get
            {
                if (this.BodyType == VoltBodyType.Invalid)
                    throw new InvalidOperationException();
                return this.BodyType == VoltBodyType.Static;
            }
        }

        /// <summary>
        /// If we're doing historical queries or tests, the body may have since
        /// been removed from the world.
        /// </summary>
        public bool IsInWorld { get { return this.World != null; } }

        // Some basic properties are stored in an internal mutable
        // record to avoid code redundancy when performing conversions
        public Vec2 Position
        {
            get { return this.currentState.position; }
            private set { this.currentState.position = value; }
        }

        public Vec2 Facing
        {
            get { return this.currentState.facing; }
            set { this.currentState.facing = value; }
        }

        public VoltAABB AABB
        {
            get { return this.currentState.aabb; }
            private set { this.currentState.aabb = value; }
        }

#if DEBUG
        internal bool IsInitialized { get; set; }
#endif

        /// <summary>
        /// For attaching arbitrary data to this body.
        /// </summary>
        public object UserData { get; set; }

        public VoltWorld World { get; private set; }
        public VoltBodyType BodyType { get; private set; }
        public VoltCollisionFilter CollisionFilter { private get; set; }

        /// <summary>
        /// Current angle in radians.
        /// </summary>
        public float Angle { get; set; }

        public Vec2 LinearVelocity { get; set; }
        public float AngularVelocity { get; set; }

        public Vec2 Force { get; private set; }
        public float Torque { get; private set; }

        public float Mass { get; private set; }
        public float Inertia { get; private set; }
        public float InvMass { get; private set; }
        public float InvInertia { get; private set; }

        internal Vec2 BiasVelocity { get; private set; }
        internal float BiasRotation { get; private set; }

        // Used for broadphase structures
        internal int ProxyId { get; set; }

        internal VoltShape[] shapes;
        internal int shapeCount;

        private bool collided;
        private VoltBody collidedBody;

        private HistoryBuffer history;
        private HistoryRecord currentState;

        #region Manipulation
        public void AddTorque(float torque)
        {
            this.Torque += torque;
        }

        public void AddForce(Vec2 force)
        {
            this.Force += force;
        }

        public void AddForce(Vec2 force, Vec2 point)
        {
            this.Force += force;
            this.Torque += VoltMath.Cross(this.Position - point, force);
        }

        public void Set(Vec2 position, float radians)
        {
            this.Position = position;
            this.Angle = radians;
            this.Facing = VoltMath.Polar(radians);
            this.OnPositionUpdated();
        }
        #endregion

        #region Tests
        /// <summary>
        /// Checks if an AABB overlaps with our AABB.
        /// </summary>
        internal bool QueryAABBOnly(
          VoltAABB worldBounds,
          int ticksBehind)
        {
            HistoryRecord record = this.GetState(ticksBehind);

            // AABB check done in world space (because it keeps changing)
            return record.aabb.Intersect(worldBounds);
        }

        /// <summary>
        /// Checks if a point is contained in this body. 
        /// Begins with AABB checks unless bypassed.
        /// </summary>
        internal bool QueryPoint(
          Vec2 point,
          int ticksBehind,
          bool bypassAABB = false)
        {
            HistoryRecord record = this.GetState(ticksBehind);

            // AABB check done in world space (because it keeps changing)
            if (bypassAABB == false)
                if (record.aabb.QueryPoint(point) == false)
                    return false;

            // Actual query on shapes done in body space
            Vec2 bodySpacePoint = record.WorldToBodyPoint(point);
            for (int i = 0; i < this.shapeCount; i++)
                if (this.shapes[i].QueryPoint(bodySpacePoint))
                    return true;
            return false;
        }

        /// <summary>
        /// Checks if a circle overlaps with this body. 
        /// Begins with AABB checks.
        /// </summary>
        internal bool QueryCircle(
          Vec2 origin,
          float radius,
          int ticksBehind,
          bool bypassAABB = false)
        {
            HistoryRecord record = this.GetState(ticksBehind);

            // AABB check done in world space (because it keeps changing)
            if (bypassAABB == false)
                if (record.aabb.QueryCircleApprox(origin, radius) == false)
                    return false;

            // Actual query on shapes done in body space
            Vec2 bodySpaceOrigin = record.WorldToBodyPoint(origin);
            for (int i = 0; i < this.shapeCount; i++)
                if (this.shapes[i].QueryCircle(bodySpaceOrigin, radius))
                    return true;
            return false;
        }

        /// <summary>
        /// Performs a ray cast check on this body. 
        /// Begins with AABB checks.
        /// </summary>
        internal bool RayCast(
          ref VoltRayCast ray,
          ref VoltRayResult result,
          int ticksBehind,
          bool bypassAABB = false)
        {
            HistoryRecord record = this.GetState(ticksBehind);

            // AABB check done in world space (because it keeps changing)
            if (bypassAABB == false)
                if (record.aabb.RayCast(ref ray) == false)
                    return false;

            // Actual tests on shapes done in body space
            VoltRayCast bodySpaceRay = record.WorldToBodyRay(ref ray);
            for (int i = 0; i < this.shapeCount; i++)
                if (this.shapes[i].RayCast(ref bodySpaceRay, ref result))
                    if (result.IsContained)
                        return true;

            // We need to convert the results back to world space to be any use
            // (Doesn't matter if we were contained since there will be no normal)
            if (result.Body == this)
                result.normal = record.BodyToWorldDirection(result.normal);
            return result.IsValid;
        }

        /// <summary>
        /// Performs a circle cast check on this body. 
        /// Begins with AABB checks.
        /// </summary>
        internal bool CircleCast(
          ref VoltRayCast ray,
          float radius,
          ref VoltRayResult result,
          int ticksBehind,
          bool bypassAABB = false)
        {
            HistoryRecord record = this.GetState(ticksBehind);

            // AABB check done in world space (because it keeps changing)
            if (bypassAABB == false)
                if (record.aabb.CircleCastApprox(ref ray, radius) == false)
                    return false;

            // Actual tests on shapes done in body space
            VoltRayCast bodySpaceRay = record.WorldToBodyRay(ref ray);
            for (int i = 0; i < this.shapeCount; i++)
                if (this.shapes[i].CircleCast(ref bodySpaceRay, radius, ref result))
                    if (result.IsContained)
                        return true;

            // We need to convert the results back to world space to be any use
            // (Doesn't matter if we were contained since there will be no normal)
            if (result.Body == this)
                result.normal = record.BodyToWorldDirection(result.normal);
            return result.IsValid;
        }
        #endregion

        public VoltBody()
        {
            this.Reset();
            this.ProxyId = -1;
            
        }

        internal void InitializeDynamic(
          Vec2 position,
          float radians,
          VoltShape[] shapesToAdd)
        {
            this.Initialize(position, radians, shapesToAdd);
            this.OnPositionUpdated();
            this.ComputeDynamics();
        }

        internal void InitializeStatic(
          Vec2 position,
          float radians,
          VoltShape[] shapesToAdd)
        {
            this.Initialize(position, radians, shapesToAdd);
            this.OnPositionUpdated();
            this.SetStatic();
        }

        private void Initialize(
          Vec2 position,
          float radians,
          VoltShape[] shapesToAdd)
        {
            this.Position = position;
            this.Angle = radians;
            this.Facing = VoltMath.Polar(radians);

#if DEBUG
            for (int i = 0; i < shapesToAdd.Length; i++)
                VoltDebug.Assert(shapesToAdd[i].IsInitialized);
#endif

            if ((this.shapes == null) || (this.shapes.Length < shapesToAdd.Length))
                this.shapes = new VoltShape[shapesToAdd.Length];
            Array.Copy(shapesToAdd, this.shapes, shapesToAdd.Length);
            this.shapeCount = shapesToAdd.Length;
            for (int i = 0; i < this.shapeCount; i++)
            {
                this.shapes[i].AssignBody(this);
                AddChild(this.shapes[i]);
            }
            game.OnAfterStep += LateReset;
#if DEBUG
            this.IsInitialized = true;
#endif
        }

        internal void RunUpdate()
        {
            if (this.history != null)
                this.history.Store(this.currentState);
            this.Integrate();
            this.OnPositionUpdated();
            //Console.WriteLine(LinearVelocity);
        }

        internal void AssignWorld(VoltWorld world)
        {
            this.World = world;
        }

        internal void FreeHistory()
        {
            if ((this.World != null) && (this.history != null))
                this.World.FreeHistory(this.history);
            this.history = null;
        }

        internal void FreeShapes()
        {
            if (this.World != null)
            {
                for (int i = 0; i < this.shapeCount; i++)
                    this.World.FreeShape(this.shapes[i]);
                for (int i = 0; i < this.shapes.Length; i++)
                    this.shapes[i] = null;
            }
            this.shapeCount = 0;
        }

        /// <summary>
        /// Used for saving the body as part of another structure. The body
        /// will retain all geometry data and associated metrics, but its
        /// position, velocity, forces, and all related history will be cleared.
        /// </summary>
        internal void PartialReset()
        {
            this.history = null;
            this.currentState = default(HistoryRecord);

            this.LinearVelocity = Vec2.Zero;
            this.AngularVelocity = 0.0f;

            this.Force = Vec2.Zero;
            this.Torque = 0.0f;

            this.BiasVelocity = Vec2.Zero;
            this.BiasRotation = 0.0f;
        }

        public bool Collision(out VoltBody body)
        {
            body = collidedBody;
            return collided;
        }

        /// <summary>
        /// Full reset. Clears out all data for pooling. Call FreeShapes() first.
        /// </summary>
        private void Reset()
        {
            VoltDebug.Assert(this.shapeCount == 0);

#if DEBUG
            this.IsInitialized = false;
#endif

            this.UserData = null;
            this.World = null;
            this.BodyType = VoltBodyType.Invalid;
            this.CollisionFilter = null;

            this.Angle = 0.0f;
            this.LinearVelocity = Vec2.Zero;
            this.AngularVelocity = 0.0f;

            this.Force = Vec2.Zero;
            this.Torque = 0.0f;

            this.Mass = 0.0f;
            this.Inertia = 0.0f;
            this.InvMass = 0.0f;
            this.InvInertia = 0.0f;

            this.BiasVelocity = Vec2.Zero;
            this.BiasRotation = 0.0f;

            this.history = null;
            this.currentState = default(HistoryRecord);
        }

        #region Collision
        internal bool CanCollide(VoltBody other)
        {
            // Ignore self and static-static collisions
            if ((this == other) || (this.IsStatic && other.IsStatic))
                return false;

            if (this.CollisionFilter != null)
                return this.CollisionFilter.Invoke(this, other);
            return true;
        }

        internal void ApplyImpulse(Vec2 j, Vec2 r)
        {
            this.LinearVelocity += this.InvMass * j;
            this.AngularVelocity -= this.InvInertia * VoltMath.Cross(j, r);
        }

        internal void ApplyBias(Vec2 j, Vec2 r)
        {
            this.BiasVelocity += this.InvMass * j;
            this.BiasRotation -= this.InvInertia * VoltMath.Cross(j, r);
        }

        public void OnCollision(VoltBody other)
        {
            collided = true;
            collidedBody = other;
        }

        public void LateReset()
        {
            collided = false;
            collidedBody = null;
        }
        #endregion

        #region Transformation Shortcuts
        internal Vec2 WorldToBodyPointCurrent(Vec2 vector)
        {
            return this.currentState.WorldToBodyPoint(vector);
        }

        internal Vec2 BodyToWorldPointCurrent(Vec2 vector)
        {
            return this.currentState.BodyToWorldPoint(vector);
        }

        internal Axis BodyToWorldAxisCurrent(Axis axis)
        {
            return this.currentState.BodyToWorldAxis(axis);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Applies the current position and angle to shapes and the AABB.
        /// </summary>
        private void OnPositionUpdated()
        {
            for (int i = 0; i < this.shapeCount; i++)
                this.shapes[i].OnBodyPositionUpdated();
            this.UpdateAABB();
            x = Position.x;
            y = Position.y;
        }

        /// <summary>
        /// Builds the AABB by combining all the shape AABBs.
        /// </summary>
        private void UpdateAABB()
        {
            float top = float.NegativeInfinity;
            float right = float.NegativeInfinity;
            float bottom = float.PositiveInfinity;
            float left = float.PositiveInfinity;

            for (int i = 0; i < this.shapeCount; i++)
            {
                VoltAABB aabb = this.shapes[i].AABB;
                top = Mathf.Max(top, aabb.Top);
                right = Mathf.Max(right, aabb.Right);
                bottom = Mathf.Min(bottom, aabb.Bottom);
                left = Mathf.Min(left, aabb.Left);
            }

            this.AABB = new VoltAABB(top, bottom, left, right);
        }

        /// <summary>
        /// Computes forces and dynamics and applies them to position and angle.
        /// </summary>
        private void Integrate()
        {
            // Apply damping
            this.LinearVelocity *= this.World.Damping;
            this.AngularVelocity *= this.World.Damping;

            //if(VoltConfig.USE_GRAVITY) AddForce(new Vec2(0, VoltConfig.GRAVITY * VoltConfig.GRAVITY_MULTIPLIER));
            if (VoltConfig.USE_GRAVITY) LinearVelocity += new Vec2(0, VoltConfig.GRAVITY * VoltConfig.GRAVITY_MULTIPLIER);

            // Calculate total force and torque
            Vec2 totalForce = this.Force * this.InvMass;
            float totalTorque = this.Torque * this.InvInertia;

            // See http://www.niksula.hut.fi/~hkankaan/Homepages/gravity.html
            this.IntegrateForces(totalForce, totalTorque, 0.5f);
            this.IntegrateVelocity();
            this.IntegrateForces(totalForce, totalTorque, 0.5f);

            this.ClearForces();
        }

        private void IntegrateForces(
          Vec2 force,
          float torque,
          float mult)
        {
            this.LinearVelocity += this.World.DeltaTime * force * mult;
            this.AngularVelocity -= this.World.DeltaTime * torque * mult;
        }

        private void IntegrateVelocity()
        {
            this.Position +=
              this.World.DeltaTime * this.LinearVelocity + this.BiasVelocity;
            this.Angle +=
              this.World.DeltaTime * this.AngularVelocity + this.BiasRotation;
            this.Facing = VoltMath.Polar(this.Angle);
        }

        private void ClearForces()
        {
            this.Force = Vec2.Zero;
            this.Torque = 0.0f;
            this.BiasVelocity = Vec2.Zero;
            this.BiasRotation = 0.0f;
        }

        private void ComputeDynamics()
        {
            this.Mass = 0.0f;
            this.Inertia = 0.0f;

            for (int i = 0; i < this.shapeCount; i++)
            {
                VoltShape shape = this.shapes[i];
                if (shape.Density == 0.0f)
                    continue;
                float curMass = shape.Mass;
                float curInertia = shape.Inertia;

                this.Mass += curMass;
                this.Inertia += curMass * curInertia;
            }

            if (this.Mass < VoltConfig.MINIMUM_DYNAMIC_MASS)
            {
                throw new InvalidOperationException("Mass of dynamic too small");
            }
            else
            {
                this.InvMass = 1.0f / this.Mass;
                this.InvInertia = 1.0f / this.Inertia;
            }

            this.BodyType = VoltBodyType.Dynamic;
        }

        private void SetStatic()
        {
            this.Mass = 0.0f;
            this.Inertia = 0.0f;
            this.InvMass = 0.0f;
            this.InvInertia = 0.0f;

            this.BodyType = VoltBodyType.Static;
        }
        #endregion

        #region Debug
#if UNITY && DEBUG
    public void GizmoDraw(
      Color edgeColor,
      Color normalColor,
      Color bodyOriginColor,
      Color shapeOriginColor,
      Color bodyAabbColor,
      Color shapeAabbColor,
      float normalLength)
    {
      Color current = Gizmos.color;

      // Draw origin
      Gizmos.color = bodyOriginColor;
      Gizmos.DrawWireSphere(this.Position, 0.1f);

      // Draw facing
      Gizmos.color = normalColor;
      Gizmos.DrawLine(
        this.Position,
        this.Position + this.Facing * normalLength);

      this.AABB.GizmoDraw(bodyAabbColor);

      for (int i = 0; i < this.shapeCount; i++)
        this.shapes[i].GizmoDraw(
          edgeColor,
          normalColor,
          shapeOriginColor,
          shapeAabbColor,
          normalLength);

      Gizmos.color = current;
    }

    public void GizmoDrawHistory(Color aabbColor)
    {
      Color current = Gizmos.color;

      if (this.history != null)
        foreach (HistoryRecord record in this.history.GetValues())
          record.aabb.GizmoDraw(aabbColor);

      Gizmos.color = current;
    }
#endif
        #endregion
    }
}