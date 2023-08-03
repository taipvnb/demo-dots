# Changelog
All notable changes to this package will be documented in this file. The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)

## [3.1.6] - 2023-4-11
- Changed seperation algorithm
- Added weight property to seperation
- Fixed NavMesh Area Mask correctly work with no sequential layers

## [3.1.5] - 2023-3-29
- Added NavMeshSurface surface baker and now it can be baked in subscene
- Added sample scene low level sonar avoidance to show its usage
- Added enable/disable to AgentNavMeshAuthoring and AgentAvoidAuthoring
- Chanded AgentSonarAvoid and NavMeshPath is now IEnableableComponent
- Fixed support for entities 1.0.0-preview.65
- Fixed acceleration correctling working with huge values
- Fixed sonar avoidance quality regression from 3.1

## [3.1.4] - 2023-2-22
- Changed com.unity.entities package version from 1.0.0-pre.15 to 1.0.0-pre.44
- Fixed AgenAuthroing.Stop to correctly set velocity to zero

## [3.1.3] - 2023-2-16
- Added support for RectTransform

## [3.1.2] - 2023-2-10
- Added SetDestinationDeferred to agent
- Fixed navmesh area mask editor property work correctly
- Changed agent capacity automatically resizing, removed AgentCapacity property for SpatialPartitioningSettings
- Changed gizmos system to be in the same group AgentGizmosSystemGroup

## [3.1.1] - 2023-2-07
- Fixed compilation issue as one of the assembly was not set to Editor

## [3.1.0] - 2023-1-31
- Added new feature to local avoidance `Walls` that accounts for navmesh.
- Added new property to AgentNavMeshAuthoring UseWalls.
- Changed standing agents now puch each other.
- Fixed local avoidance gizmos drawing.
- Fixed then desination either above or below agent would result in error.
- Fixed path failure case then it is out of nodes and path is in progress.

## [3.0.6] - 2023-1-11
- Fixed NavMesh path sometimes discarding destination
- Fixed error drop when selecting agent in subscene "The targets array should not be used inside OnSceneGUI or OnPreviewGUI. Use the single target property instead.
UnityEngine.GUIUtility:ProcessEvent (int,intptr,bool&)"

## [3.0.5] - 2022-12-26
- Fixed NavMeshAgent correctly stop if path destination can not be mapped to navmesh
- Fixed that even with OutOfNodes still returns best possible path
- Added NavMeshPath failed state and also prints the error in editor
- Added NavMeshAgent/NavMeshPath added new property MappingExtent that allows controling the maximum distance the agent will be mapped
- Added documentation links to components and package
- Changed documentation to hidden folder as now it is on webpage

## [3.0.4] - 2022-12-23
- Fixed NavMeshAgent correctly handle partial paths (Paths where destination can not be reached)
- Fixed few more cases where NavMesh update would result in "Any jobs using NavMeshQuery must be completed before we mutate the NavMesh."
- Fixed NavMeshAgent in some cases reusing path from other agent
- Changed Zerg scene camera to be centered around controllable units

## [3.0.3] - 2022-12-17
- Added to EntityBehaviour OnEnable and OnDisable
- Added error message box to AgentNavMeshAuthoring, if game objects also has NavMeshObstacle
- Added SetDestination method to AgentAuthoring
- Changed that if agent is not near any NavMesh it will throw error instead moved to the center of the world
- Fixed few cases where NavMesh update would result in "Any jobs using NavMeshQuery must be completed before we mutate the NavMesh." 

## [3.0.2] - 2022-12-15
- Fixed NavMesh at the end of destination throwing error `System.IndexOutOfRangeException: Index {0} is out of range of '{1}' Length`.
- Fixed transform sync from game object to entity not override transform in most calls.

## [3.0.1] - 2022-12-08
- Added correct documentation
- Added com.unity.modules.ui dependency as samples uses ui
- Removed second navmesh surface from zerg samples

## [3.0.0] - 2022-11-30
- Release as Agents Navigation

## [2.0.0] - 2022-06-9
- Changing velocity avoidance with new smart algorithm
- Changing package to use new Package Manager workflow
- Updating documentation to be more clear and reflect new API changes
- Adding zerg sample

## [1.0.3] - 2022-05-14
- Adding new demo scene "8 - Jobified Boids Navmesh Demo"

## [1.0.2] - 2022-03-19
- Fixing memory leaks in demo scenes

## [1.0.1] - 2022-03-08
- Updated jobs demo to not use physics and small bug fix

## [1.0.0] - 2022-02-22
- Package released