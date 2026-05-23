using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgenteControlador : MonoBehaviour
{
    public enum TipoAgente { Humano, Pet, Monstro }

    [Header("Configurações Principais")]
    [Tooltip("Escolha o tipo de agente para aplicar as regras automaticamente.")]
    public TipoAgente tipoDoAgente;
    
    [Tooltip("O Transform (alvo) que o agente deve seguir.")]
    public Transform alvo;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ConfigurarAgente();
    }

    void Update()
    {
        // Segue o alvo a cada frame, se ele existir
        if (alvo != null && agent.isOnNavMesh)
        {
            agent.SetDestination(alvo.position);
        }
    }

    // Permite que as configurações atualizem automaticamente se você mudar o tipo no Editor durante o Play
    void OnValidate()
    {
        if (Application.isPlaying && agent != null)
        {
            ConfigurarAgente();
        }
    }

    /// <summary>
    /// Configura as dimensões do agente e quais áreas do NavMesh ele pode acessar.
    /// </summary>
    public void ConfigurarAgente()
    {
        // Resetamos a máscara para pegar sempre a área padrão "Walkable"
        int areaMask = 1 << NavMesh.GetAreaFromName("Walkable");
return;
        switch (tipoDoAgente)
        {
            case TipoAgente.Humano:
                // Dimensões do Humano (Ref: 1.9m)
                agent.height = 1.9f;
                agent.radius = 0.3f;
                
                // Humano passa por Portas e Escadas. (Não passa em PassagemBaixa, Parede ou Abismo)
                areaMask |= (1 << GetAreaIndex("Porta"));
                areaMask |= (1 << GetAreaIndex("Escada"));
                
                agent.autoTraverseOffMeshLink = false; // Não pula abismos
                break;

            case TipoAgente.Pet:
                // Dimensões do Pet
                agent.height = 0.5f;
                agent.radius = 0.15f;
                
                // Pet passa por debaixo de mesas, rampas (Walkable padrão) e paredes.
                areaMask |= (1 << GetAreaIndex("PassagemBaixa"));
                areaMask |= (1 << GetAreaIndex("Parede"));
                
                agent.autoTraverseOffMeshLink = false; // Não pula abismos
                break;

            case TipoAgente.Monstro:
                // Dimensões do Monstro
                agent.height = 2.5f;
                agent.radius = 0.8f; 
                
                // Monstro usa apenas o chão padrão (Walkable) e pode usar os links de pulo (Abismo).
                areaMask |= (1 << GetAreaIndex("Abismo"));
                
                agent.autoTraverseOffMeshLink = true; // Permite usar o NavMeshLink do abismo
                break;
        }

        // Aplica a máscara de navegação ao agente
        agent.areaMask = areaMask;
        
        // Ajusta a cápsula visualmente para refletir o tamanho do agente
        CapsuleCollider capsula = GetComponent<CapsuleCollider>();
        if (capsula != null)
        {
            capsula.height = agent.height;
            capsula.radius = agent.radius;
            capsula.center = new Vector3(0, agent.height / 2f, 0);
        }
    }

    /// <summary>
    /// Função auxiliar para buscar o índice da área de forma segura.
    /// </summary>
    private int GetAreaIndex(string nomeArea)
    {
        int index = NavMesh.GetAreaFromName(nomeArea);
        if (index == -1)
        {
            Debug.LogWarning($"Atenção: Área '{nomeArea}' não foi criada na janela de Navigation!");
            return 0; // Se não achar, não adiciona permissão extra
        }
        return index;
    }
}