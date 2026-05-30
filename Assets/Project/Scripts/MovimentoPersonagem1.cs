using UnityEngine;

/// <summary>
/// Movimento do Personagem 1 (Soldado).
/// WASD = andar/correr | Shift = correr | Mouse X = girar o personagem
/// Só funciona quando este personagem está ativo (tecla 1).
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class MovimentoPersonagem1 : MonoBehaviour
{
    public float velocidade = 5f;
    public float bonusCorrer = 3.5f;
    public float gravidade = 9.8f;
    public float rotacaoMouse = 3f;

    CharacterController characterController;
    Animator animator;
    CombatePersonagem1 combate;
    bool ativo;

    public void Configurar(CombatePersonagem1 combatePersonagem)
    {
        combate = combatePersonagem;
    }

    public void DefinirAtivo(bool personagemAtivo)
    {
        ativo = personagemAtivo;

        if (!ativo && animator != null)
        {
            animator.SetBool("run", false);
            animator.SetBool("sprint", false);
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!ativo)
        {
            if (animator != null)
            {
                animator.SetBool("run", false);
                animator.SetBool("sprint", false);
            }

            characterController.Move(Vector3.down * gravidade * Time.deltaTime);
            return;
        }

        if (combate != null && combate.EstaEmAnimacao)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool correr = Input.GetKey(KeyCode.LeftShift);

        transform.Rotate(0f, Input.GetAxis("Mouse X") * rotacaoMouse, 0f);

        float velocidadeAtual = velocidade + (correr ? bonusCorrer : 0f);
        Vector3 movimento = Vector3.down * gravidade * Time.deltaTime;

        if (horizontal != 0f || vertical != 0f)
        {
            Vector3 direcaoHorizontal = (transform.forward * vertical + transform.right * horizontal).normalized;
            movimento += direcaoHorizontal * velocidadeAtual * Time.deltaTime;
        }

        characterController.Move(movimento);

        if (animator != null)
        {
            bool seMovendo = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z).magnitude > 0.1f;
            animator.SetBool("run", seMovendo);
            animator.SetBool("sprint", seMovendo && correr);
        }
    }
}
