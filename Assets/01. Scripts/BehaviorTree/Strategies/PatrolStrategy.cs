using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolStrategy : IStrategy
{
    // 길찾기하는데 필요한거
    public readonly Transform entity;
    public readonly NavMeshAgent agent;
    public readonly List<Transform> points;
    public int currentIndex; // 포인트 추적
    private bool _isPathCalculating;

    public PatrolStrategy(Transform entity, NavMeshAgent agent, List<Transform> points)
    {
        this.entity = entity;
        this.agent = agent;
        this.points = points;
    }

    public Status Process()
    {      
        Transform target = points[currentIndex];
        agent.stoppingDistance = 0;

        if (currentIndex == 0) 
            agent.SetDestination(target.position);

        float distance = Vector3.Distance(entity.position, target.position);
        if (distance <= 0.1f) // !agent.pathPending && 
        {
            ++currentIndex;
            if (currentIndex == points.Count) return Status.Success; // 다 돌았으면 성공 리턴

            target = points[currentIndex];
            agent.SetDestination(target.position);
        }
        return Status.Running;
    }

    public void Reset()
    {
        currentIndex = 0;
    }
}
