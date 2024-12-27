using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolStrategy : IStrategy
{
    // ��ã���ϴµ� �ʿ��Ѱ�
    public readonly Transform entity;
    public readonly NavMeshAgent agent;
    public readonly List<Transform> points;
    public int currentIndex; // ����Ʈ ����
    private bool _isPathCalculating;

    public PatrolStrategy(Transform entity, NavMeshAgent agent, List<Transform> points)
    {
        this.entity = entity;
        this.agent = agent;
        this.points = points;
    }

    public Status Process()
    {
        if(currentIndex >= points.Count) return Status.Success; // �� �������� ���� ����

        Transform target = points[currentIndex];
        agent.stoppingDistance = 0;

        if (currentIndex == 0) agent.SetDestination(target.position);

        if (!agent.pathPending && agent.remainingDistance <= 0.1f)
        {
            ++currentIndex;
            agent.SetDestination(target.position);
        }

        /*Transform target = points[currentIndex];
        agent.SetDestination(target.position);
        agent.stoppingDistance = 0;

        if(_isPathCalculating && agent.remainingDistance <= 0.1f)    
        {
            ++currentIndex;
            _isPathCalculating = false;
        }

        if (agent.pathPending)
            _isPathCalculating = true;*/

        // ��� ��� �� ���� ������ 

        return Status.Running;
    }

    public void Reset()
    {
        currentIndex = 0;
    }
}
