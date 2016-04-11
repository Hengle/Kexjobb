using System.Collections;
using System.Collections.Generic;

namespace RVO
{
	public class RVOAgent
	{
		// From current position & current (preferred) velocity, compute target velocity.
		// If/how this velocity is applied is up to the user.
		public Vector2 Position;
		public Vector2 PreferredVelocity;
		public Vector2 TargetVelocity;

		public float MaxSpeed;
		public float Radius;

		private IList<KeyValuePair<float, RVOAgent>> agentNeighbors_ = new List<KeyValuePair<float, RVOAgent>>();
		private int maxNeighbors_ = 10;
		private float neighborDist_ = 5.0f;

		private IList<KeyValuePair<float, Obstacle>> obstacleNeighbors_ = new List<KeyValuePair<float, Obstacle>>();
		private float timeHorizon_ = 5.0f;
		private float timeHorizonObst_ = 5.0f;

		private IList<Line> orcaLines_ = new List<Line>();

		public void computeNeighbors()
		{
			obstacleNeighbors_.Clear();
			float rangeSq = RVOMath.sqr(timeHorizonObst_ * MaxSpeed + Radius);
			Simulator.Instance.kdTree_.computeObstacleNeighbors(this, rangeSq);

			agentNeighbors_.Clear();
			if (maxNeighbors_ > 0)
			{
				rangeSq = RVOMath.sqr(neighborDist_);
				Simulator.Instance.kdTree_.computeAgentNeighbors(this, ref rangeSq);
			}
		}

		/* Search for the best target velocity. */
		public void computeTargetVelocity()
		{
			orcaLines_.Clear();

			float invTimeHorizonObst = 1.0f / timeHorizonObst_;

			/* Create obstacle ORCA lines. */
			for (int i = 0; i < obstacleNeighbors_.Count; ++i)
			{

				Obstacle obstacle1 = obstacleNeighbors_[i].Value;
				Obstacle obstacle2 = obstacle1.nextObstacle;

				Vector2 relativePosition1 = obstacle1.point_ - Position;
				Vector2 relativePosition2 = obstacle2.point_ - Position;

				/* 
				 * Check if velocity obstacle of obstacle is already taken care of by
				 * previously constructed obstacle ORCA lines.
				 */
				bool alreadyCovered = false;

				for (int j = 0; j < orcaLines_.Count; ++j)
				{
					if (RVOMath.det(invTimeHorizonObst * relativePosition1 - orcaLines_[j].point, orcaLines_[j].direction) - invTimeHorizonObst * Radius >= -RVOMath.RVO_EPSILON && RVOMath.det(invTimeHorizonObst * relativePosition2 - orcaLines_[j].point, orcaLines_[j].direction) - invTimeHorizonObst * Radius >= -RVOMath.RVO_EPSILON)
					{
						alreadyCovered = true;
						break;
					}
				}

				if (alreadyCovered)
				{
					continue;
				}

				/* Not yet covered. Check for collisions. */

				float distSq1 = RVOMath.absSq(relativePosition1);
				float distSq2 = RVOMath.absSq(relativePosition2);

				float radiusSq = RVOMath.sqr(Radius);

				Vector2 obstacleVector = obstacle2.point_ - obstacle1.point_;
				float s = (-relativePosition1 * obstacleVector) / RVOMath.absSq(obstacleVector);
				float distSqLine = RVOMath.absSq(-relativePosition1 - s * obstacleVector);

				Line line;

				if (s < 0 && distSq1 <= radiusSq)
				{
					/* Collision with left vertex. Ignore if non-convex. */
					if (obstacle1.isConvex_)
					{
						line.point = new Vector2(0, 0);
						line.direction = RVOMath.normalize(new Vector2(-relativePosition1.y(), relativePosition1.x()));
						orcaLines_.Add(line);
					}
					continue;
				}
				else if (s > 1 && distSq2 <= radiusSq)
				{
					/* Collision with right vertex. Ignore if non-convex 
					 * or if it will be taken care of by neighoring obstace */
					if (obstacle2.isConvex_ && RVOMath.det(relativePosition2, obstacle2.unitDir_) >= 0)
					{
						line.point = new Vector2(0, 0);
						line.direction = RVOMath.normalize(new Vector2(-relativePosition2.y(), relativePosition2.x()));
						orcaLines_.Add(line);
					}
					continue;
				}
				else if (s >= 0 && s < 1 && distSqLine <= radiusSq)
				{
					/* Collision with obstacle segment. */
					line.point = new Vector2(0, 0);
					line.direction = -obstacle1.unitDir_;
					orcaLines_.Add(line);
					continue;
				}

				/* 
				 * No collision.  
				 * Compute legs. When obliquely viewed, both legs can come from a single
				 * vertex. Legs extend cut-off line when nonconvex vertex.
				 */

				Vector2 leftLegDirection, rightLegDirection;

				if (s < 0 && distSqLine <= radiusSq)
				{
					/*
					 * Obstacle viewed obliquely so that left vertex
					 * defines velocity obstacle.
					 */
					if (!obstacle1.isConvex_)
					{
						/* Ignore obstacle. */
						continue;
					}

					obstacle2 = obstacle1;

					float leg1 = RVOMath.sqrt(distSq1 - radiusSq);
					leftLegDirection = new Vector2(relativePosition1.x() * leg1 - relativePosition1.y() * Radius, relativePosition1.x() * Radius + relativePosition1.y() * leg1) / distSq1;
					rightLegDirection = new Vector2(relativePosition1.x() * leg1 + relativePosition1.y() * Radius, -relativePosition1.x() * Radius + relativePosition1.y() * leg1) / distSq1;
				}
				else if (s > 1 && distSqLine <= radiusSq)
				{
					/*
					 * Obstacle viewed obliquely so that
					 * right vertex defines velocity obstacle.
					 */
					if (!obstacle2.isConvex_)
					{
						/* Ignore obstacle. */
						continue;
					}

					obstacle1 = obstacle2;

					float leg2 = RVOMath.sqrt(distSq2 - radiusSq);
					leftLegDirection = new Vector2(relativePosition2.x() * leg2 - relativePosition2.y() * Radius, relativePosition2.x() * Radius + relativePosition2.y() * leg2) / distSq2;
					rightLegDirection = new Vector2(relativePosition2.x() * leg2 + relativePosition2.y() * Radius, -relativePosition2.x() * Radius + relativePosition2.y() * leg2) / distSq2;
				}
				else
				{
					/* Usual situation. */
					if (obstacle1.isConvex_)
					{
						float leg1 = RVOMath.sqrt(distSq1 - radiusSq);
						leftLegDirection = new Vector2(relativePosition1.x() * leg1 - relativePosition1.y() * Radius, relativePosition1.x() * Radius + relativePosition1.y() * leg1) / distSq1;
					}
					else
					{
						/* Left vertex non-convex; left leg extends cut-off line. */
						leftLegDirection = -obstacle1.unitDir_;
					}

					if (obstacle2.isConvex_)
					{
						float leg2 = RVOMath.sqrt(distSq2 - radiusSq);
						rightLegDirection = new Vector2(relativePosition2.x() * leg2 + relativePosition2.y() * Radius, -relativePosition2.x() * Radius + relativePosition2.y() * leg2) / distSq2;
					}
					else
					{
						/* Right vertex non-convex; right leg extends cut-off line. */
						rightLegDirection = obstacle1.unitDir_;
					}
				}

				/* 
				 * Legs can never point into neighboring edge when convex vertex,
				 * take cutoff-line of neighboring edge instead. If velocity projected on
				 * "foreign" leg, no constraint is added. 
				 */

				Obstacle leftNeighbor = obstacle1.prevObstacle;

				bool isLeftLegForeign = false;
				bool isRightLegForeign = false;

				if (obstacle1.isConvex_ && RVOMath.det(leftLegDirection, -leftNeighbor.unitDir_) >= 0.0f)
				{
					/* Left leg points into obstacle. */
					leftLegDirection = -leftNeighbor.unitDir_;
					isLeftLegForeign = true;
				}

				if (obstacle2.isConvex_ && RVOMath.det(rightLegDirection, obstacle2.unitDir_) <= 0.0f)
				{
					/* Right leg points into obstacle. */
					rightLegDirection = obstacle2.unitDir_;
					isRightLegForeign = true;
				}

				/* Compute cut-off centers. */
				Vector2 leftCutoff = invTimeHorizonObst * (obstacle1.point_ - Position);
				Vector2 rightCutoff = invTimeHorizonObst * (obstacle2.point_ - Position);
				Vector2 cutoffVec = rightCutoff - leftCutoff;

				/* Project current velocity on velocity obstacle. */

				/* Check if current velocity is projected on cutoff circles. */
				float t = (obstacle1 == obstacle2 ? 0.5f : (TargetVelocity - leftCutoff) * cutoffVec) / RVOMath.absSq(cutoffVec);
				float tLeft = ((TargetVelocity - leftCutoff) * leftLegDirection);
				float tRight = ((TargetVelocity - rightCutoff) * rightLegDirection);

				if ((t < 0.0f && tLeft < 0.0f) || (obstacle1 == obstacle2 && tLeft < 0.0f && tRight < 0.0f))
				{
					/* Project on left cut-off circle. */
					Vector2 unitW = RVOMath.normalize(TargetVelocity - leftCutoff);

					line.direction = new Vector2(unitW.y(), -unitW.x());
					line.point = leftCutoff + Radius * invTimeHorizonObst * unitW;
					orcaLines_.Add(line);
					continue;
				}
				else if (t > 1.0f && tRight < 0.0f)
				{
					/* Project on right cut-off circle. */
					Vector2 unitW = RVOMath.normalize(TargetVelocity - rightCutoff);

					line.direction = new Vector2(unitW.y(), -unitW.x());
					line.point = rightCutoff + Radius * invTimeHorizonObst * unitW;
					orcaLines_.Add(line);
					continue;
				}

				/* 
				 * Project on left leg, right leg, or cut-off line, whichever is closest
				 * to velocity.
				 */
				float distSqCutoff = ((t < 0.0f || t > 1.0f || obstacle1 == obstacle2) ? float.PositiveInfinity : RVOMath.absSq(TargetVelocity - (leftCutoff + t * cutoffVec)));
				float distSqLeft = ((tLeft < 0.0f) ? float.PositiveInfinity : RVOMath.absSq(TargetVelocity - (leftCutoff + tLeft * leftLegDirection)));
				float distSqRight = ((tRight < 0.0f) ? float.PositiveInfinity : RVOMath.absSq(TargetVelocity - (rightCutoff + tRight * rightLegDirection)));

				if (distSqCutoff <= distSqLeft && distSqCutoff <= distSqRight)
				{
					/* Project on cut-off line. */
					line.direction = -obstacle1.unitDir_;
					line.point = leftCutoff + Radius * invTimeHorizonObst * new Vector2(-line.direction.y(), line.direction.x());
					orcaLines_.Add(line);
					continue;
				}
				else if (distSqLeft <= distSqRight)
				{
					/* Project on left leg. */
					if (isLeftLegForeign)
					{
						continue;
					}

					line.direction = leftLegDirection;
					line.point = leftCutoff + Radius * invTimeHorizonObst * new Vector2(-line.direction.y(), line.direction.x());
					orcaLines_.Add(line);
					continue;
				}
				else
				{
					/* Project on right leg. */
					if (isRightLegForeign)
					{
						continue;
					}

					line.direction = -rightLegDirection;
					line.point = rightCutoff + Radius * invTimeHorizonObst * new Vector2(-line.direction.y(), line.direction.x());
					orcaLines_.Add(line);
					continue;
				}
			}

			int numObstLines = orcaLines_.Count;

			float invTimeHorizon = 1.0f / timeHorizon_;

			/* Create agent ORCA lines. */
			for (int i = 0; i < agentNeighbors_.Count; ++i)
			{
				RVOAgent other = agentNeighbors_[i].Value;
				Vector2 relativePosition = other.Position - Position;
				Vector2 relativeVelocity = TargetVelocity - other.TargetVelocity;
				float distSq = RVOMath.absSq(relativePosition);
				float combinedRadius = Radius + other.Radius;
				float combinedRadiusSq = RVOMath.sqr(combinedRadius);

				Line line;
				Vector2 u;

				if (distSq > combinedRadiusSq)
				{
					/* No collision. */
					Vector2 w = relativeVelocity - invTimeHorizon * relativePosition;
					/* Vector from cutoff center to relative velocity. */
					float wLengthSq = RVOMath.absSq(w);

					float dotProduct1 = w * relativePosition;

					if (dotProduct1 < 0.0f && RVOMath.sqr(dotProduct1) > combinedRadiusSq * wLengthSq)
					{
						/* Project on cut-off circle. */
						float wLength = RVOMath.sqrt(wLengthSq);
						Vector2 unitW = w / wLength;

						line.direction = new Vector2(unitW.y(), -unitW.x());
						u = (combinedRadius * invTimeHorizon - wLength) * unitW;
					}
					else
					{
						/* Project on legs. */
						float leg = RVOMath.sqrt(distSq - combinedRadiusSq);

						if (RVOMath.det(relativePosition, w) > 0.0f)
						{
							/* Project on left leg. */
							line.direction = new Vector2(relativePosition.x() * leg - relativePosition.y() * combinedRadius, relativePosition.x() * combinedRadius + relativePosition.y() * leg) / distSq;
						}
						else
						{
							/* Project on right leg. */
							line.direction = -new Vector2(relativePosition.x() * leg + relativePosition.y() * combinedRadius, -relativePosition.x() * combinedRadius + relativePosition.y() * leg) / distSq;
						}

						float dotProduct2 = relativeVelocity * line.direction;

						u = dotProduct2 * line.direction - relativeVelocity;
					}
				}
				else
				{
					/* Collision. Project on cut-off circle of time timeStep. */
					float invTimeStep = 1.0f / Simulator.Instance.timeStep_;

					/* Vector from cutoff center to relative velocity. */
					Vector2 w = relativeVelocity - invTimeStep * relativePosition;

					float wLength = RVOMath.abs(w);
					Vector2 unitW = w / wLength;

					line.direction = new Vector2(unitW.y(), -unitW.x());
					u = (combinedRadius * invTimeStep - wLength) * unitW;
				}

				line.point = TargetVelocity + 0.5f * u;
				orcaLines_.Add(line);
			}


			int lineFail = linearProgram2(orcaLines_, MaxSpeed, PreferredVelocity, false, ref TargetVelocity);

			if (lineFail < orcaLines_.Count)
			{
				linearProgram3(orcaLines_, numObstLines, lineFail, MaxSpeed, ref TargetVelocity);
			}

		}

		public void insertAgentNeighbor(RVOAgent agent, ref float rangeSq)
		{
			if (this != agent)
			{
				float distSq = RVOMath.absSq(Position - agent.Position);

				if (distSq < rangeSq)
				{
					if (agentNeighbors_.Count < maxNeighbors_)
					{
						agentNeighbors_.Add(new KeyValuePair<float, RVOAgent>(distSq, agent));
					}
					int i = agentNeighbors_.Count - 1;
					while (i != 0 && distSq < agentNeighbors_[i - 1].Key)
					{
						agentNeighbors_[i] = agentNeighbors_[i - 1];
						--i;
					}
					agentNeighbors_[i] = new KeyValuePair<float, RVOAgent>(distSq, agent);

					if (agentNeighbors_.Count == maxNeighbors_)
					{
						rangeSq = agentNeighbors_[agentNeighbors_.Count - 1].Key;
					}
				}
			}
		}

		public void insertObstacleNeighbor(Obstacle obstacle, float rangeSq)
		{
			Obstacle nextObstacle = obstacle.nextObstacle;

			float distSq = RVOMath.distSqPointLineSegment(obstacle.point_, nextObstacle.point_, Position);

			if (distSq < rangeSq)
			{
				obstacleNeighbors_.Add(new KeyValuePair<float, Obstacle>(distSq, obstacle));

				int i = obstacleNeighbors_.Count - 1;
				while (i != 0 && distSq < obstacleNeighbors_[i - 1].Key)
				{
					obstacleNeighbors_[i] = obstacleNeighbors_[i - 1];
					--i;
				}
				obstacleNeighbors_[i] = new KeyValuePair<float, Obstacle>(distSq, obstacle);
			}
		}

		bool linearProgram1(IList<Line> lines, int lineNo, float radius, Vector2 optVelocity, bool directionOpt, ref Vector2 result)
		{
			float dotProduct = lines[lineNo].point * lines[lineNo].direction;
			float discriminant = RVOMath.sqr(dotProduct) + RVOMath.sqr(radius) - RVOMath.absSq(lines[lineNo].point);

			if (discriminant < 0.0f)
			{
				/* Max speed circle fully invalidates line lineNo. */
				return false;
			}

			float sqrtDiscriminant = RVOMath.sqrt(discriminant);
			float tLeft = -dotProduct - sqrtDiscriminant;
			float tRight = -dotProduct + sqrtDiscriminant;

			for (int i = 0; i < lineNo; ++i)
			{
				float denominator = RVOMath.det(lines[lineNo].direction, lines[i].direction);
				float numerator = RVOMath.det(lines[i].direction, lines[lineNo].point - lines[i].point);

				if (RVOMath.fabs(denominator) <= RVOMath.RVO_EPSILON)
				{
					/* Lines lineNo and i are (almost) parallel. */
					if (numerator < 0.0f)
					{
						return false;
					}
					else
					{
						continue;
					}
				}

				float t = numerator / denominator;

				if (denominator >= 0.0f)
				{
					/* Line i bounds line lineNo on the right. */
					if (t < tRight)
						tRight = t;
				}
				else
				{
					/* Line i bounds line lineNo on the left. */
					if (t > tLeft)
						tLeft = t;
				}

				if (tLeft > tRight)
				{
					return false;
				}
			}

			if (directionOpt)
			{
				/* Optimize direction. */
				if (optVelocity * lines[lineNo].direction > 0.0f)
				{
					/* Take right extreme. */
					result = lines[lineNo].point + tRight * lines[lineNo].direction;
				}
				else
				{
					/* Take left extreme. */
					result = lines[lineNo].point + tLeft * lines[lineNo].direction;
				}
			}
			else
			{
				/* Optimize closest point. */
				float t = lines[lineNo].direction * (optVelocity - lines[lineNo].point);

				if (t < tLeft)
				{
					result = lines[lineNo].point + tLeft * lines[lineNo].direction;
				}
				else if (t > tRight)
				{
					result = lines[lineNo].point + tRight * lines[lineNo].direction;
				}
				else
				{
					result = lines[lineNo].point + t * lines[lineNo].direction;
				}
			}

			return true;
		}

		int linearProgram2(IList<Line> lines, float radius, Vector2 optVelocity, bool directionOpt, ref Vector2 result)
		{
			if (directionOpt)
			{
				/* 
				 * Optimize direction. Note that the optimization velocity is of unit
				 * length in this case.
				 */
				result = optVelocity * radius;
			}
			else if (RVOMath.absSq(optVelocity) > RVOMath.sqr(radius))
			{
				/* Optimize closest point and outside circle. */
				result = RVOMath.normalize(optVelocity) * radius;
			}
			else
			{
				/* Optimize closest point and inside circle. */
				result = optVelocity;
			}

			for (int i = 0; i < lines.Count; ++i)
			{
				if (RVOMath.det(lines[i].direction, lines[i].point - result) > 0.0f)
				{
					/* Result does not satisfy constraint i. Compute new optimal result. */
					Vector2 tempResult = result;
					if (!linearProgram1(lines, i, radius, optVelocity, directionOpt, ref result))
					{
						result = tempResult;
						return i;
					}
				}
			}

			return lines.Count;
		}

		void linearProgram3(IList<Line> lines, int numObstLines, int beginLine, float radius, ref Vector2 result)
		{
			float distance = 0.0f;

			for (int i = beginLine; i < lines.Count; ++i)
			{
				if (RVOMath.det(lines[i].direction, lines[i].point - result) > distance)
				{
					/* Result does not satisfy constraint of line i. */
					//std::vector<Line> projLines(lines.begin(), lines.begin() + numObstLines);
					IList<Line> projLines = new List<Line>();
					for (int ii = 0; ii < numObstLines; ++ii)
					{
						projLines.Add(lines[ii]);
					}

					for (int j = numObstLines; j < i; ++j)
					{
						Line line;

						float determinant = RVOMath.det(lines[i].direction, lines[j].direction);

						if (RVOMath.fabs(determinant) <= RVOMath.RVO_EPSILON)
						{
							/* Line i and line j are parallel. */
							if (lines[i].direction * lines[j].direction > 0.0f)
							{
								/* Line i and line j point in the same direction. */
								continue;
							}
							else
							{
								/* Line i and line j point in opposite direction. */
								line.point = 0.5f * (lines[i].point + lines[j].point);
							}
						}
						else
						{
							line.point = lines[i].point + (RVOMath.det(lines[j].direction, lines[i].point - lines[j].point) / determinant) * lines[i].direction;
						}

						line.direction = RVOMath.normalize(lines[j].direction - lines[i].direction);
						projLines.Add(line);
					}

					Vector2 tempResult = result;
					if (linearProgram2(projLines, radius, new Vector2(-lines[i].direction.y(), lines[i].direction.x()), true, ref result) < projLines.Count)
					{
						/* This should in principle not happen.  The result is by definition
						 * already in the feasible region of this linear program. If it fails,
						 * it is due to small floating point error, and the current result is
						 * kept.
						 */
						result = tempResult;
					}

					distance = RVOMath.det(lines[i].direction, lines[i].point - result);
				}
			}
		}
	}
}
