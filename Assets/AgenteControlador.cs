using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgenteControlador : MonoBehaviour
{
    public Transform alvo;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (alvo != null && agent.isOnNavMesh)
        {
            agent.SetDestination(alvo.position);
        }
    }
}