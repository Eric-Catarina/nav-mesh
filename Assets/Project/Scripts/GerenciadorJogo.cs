using UnityEngine;

/// <summary>
/// Coloque este script em um objeto vazio na cena (ex: "GerenciadorJogo").
/// Ele cria o segundo personagem, liga as referências e troca o controle com as teclas 1 e 2.
/// </summary>
[DefaultExecutionOrder(-100)]
public class GerenciadorJogo : MonoBehaviour
{
    [Header("Opcional — cor do Personagem 2")]
    public Material materialPersonagem2;

    Transform personagem1;
    Transform personagem2;
    MovimentoPersonagem1 movimento1;
    MovimentoPersonagem2 movimento2;
    CombatePersonagem1 combate1;
    CombatePersonagem2 combate2;
    CameraController cameraController;

    int personagemAtivo = 1;

    void Awake()
    {
        ConfigurarPersonagens();
    }

    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
        AtivarPersonagem(1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            AtivarPersonagem(1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            AtivarPersonagem(2);
    }

    void ConfigurarPersonagens()
    {
        GameObject objetoPersonagem1 = GameObject.Find("ThirdPersonController");
        if (objetoPersonagem1 == null)
        {
            Debug.LogError("GerenciadorJogo: não encontrou 'ThirdPersonController' na cena.");
            return;
        }

        objetoPersonagem1.name = "Personagem1";

        ThirdPersonController controleAntigo = objetoPersonagem1.GetComponent<ThirdPersonController>();
        if (controleAntigo != null)
            Destroy(controleAntigo);

        movimento1 = objetoPersonagem1.AddComponent<MovimentoPersonagem1>();
        combate1 = objetoPersonagem1.AddComponent<CombatePersonagem1>();
        personagem1 = objetoPersonagem1.transform;

        GameObject objetoPersonagem2 = Instantiate(objetoPersonagem1, new Vector3(6f, 0f, 2f), Quaternion.Euler(0f, 180f, 0f));
        objetoPersonagem2.name = "Personagem2";
        objetoPersonagem2.tag = "Untagged";

        Destroy(objetoPersonagem2.GetComponent<MovimentoPersonagem1>());
        Destroy(objetoPersonagem2.GetComponent<CombatePersonagem1>());

        movimento2 = objetoPersonagem2.AddComponent<MovimentoPersonagem2>();
        combate2 = objetoPersonagem2.AddComponent<CombatePersonagem2>();
        personagem2 = objetoPersonagem2.transform;

        if (materialPersonagem2 != null)
        {
            SkinnedMeshRenderer mesh = objetoPersonagem2.GetComponentInChildren<SkinnedMeshRenderer>();
            if (mesh != null)
                mesh.material = materialPersonagem2;
        }

        combate1.Configurar(combate2);
        combate2.Configurar(combate1);
        movimento1.Configurar(combate1);
        movimento2.Configurar(combate2);
    }

    void AtivarPersonagem(int numero)
    {
        personagemAtivo = numero;

        bool ativo1 = numero == 1;
        movimento1.DefinirAtivo(ativo1);
        movimento2.DefinirAtivo(!ativo1);
        combate1.DefinirAtivo(ativo1);
        combate2.DefinirAtivo(!ativo1);

        Transform alvoCamera = ativo1 ? personagem1 : personagem2;
        if (cameraController != null)
            cameraController.DefinirAlvo(alvoCamera);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
