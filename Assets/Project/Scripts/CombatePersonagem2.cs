using System.Collections;
using UnityEngine;

/// <summary>
/// Combate do Personagem 2.
/// Botão esquerdo = chute (corpo a corpo) | Botão direito = onda de energia (distância)
/// Animações: Slow Run (chute), Jump (onda), Crouched Walking (tomar hit)
/// </summary>
public class CombatePersonagem2 : MonoBehaviour
{
    const string AnimacaoChute = "Slow Run";
    const string AnimacaoOnda = "Jump";
    const string AnimacaoHit = "Crouched Walking";

    public float alcanceChute = 2.2f;
    public float alcanceOnda = 12f;
    public float tempoAnimacaoChute = 0.7f;
    public float tempoAnimacaoOnda = 0.6f;
    public float tempoAnimacaoHit = 0.9f;

    CombatePersonagem1 oponente;
    Animator animator;
    bool ativo;
    bool emAnimacao;

    public bool EstaEmAnimacao => emAnimacao;

    public void Configurar(CombatePersonagem1 oponenteCombate)
    {
        oponente = oponenteCombate;
    }

    public void DefinirAtivo(bool personagemAtivo)
    {
        ativo = personagemAtivo;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!ativo || emAnimacao)
            return;

        if (Input.GetMouseButtonDown(0))
            StartCoroutine(Chute());

        if (Input.GetMouseButtonDown(1))
            StartCoroutine(Onda());
    }

    IEnumerator Chute()
    {
        emAnimacao = true;
        animator.Play(AnimacaoChute, 0, 0f);

        CriarEfeitoChute(new Color(1f, 0.5f, 0f));

        yield return new WaitForSeconds(tempoAnimacaoChute * 0.5f);

        if (oponente != null && DistanciaAte(oponente.transform) <= alcanceChute)
            oponente.ReceberHit();

        yield return new WaitForSeconds(tempoAnimacaoChute * 0.5f);
        emAnimacao = false;
    }

    IEnumerator Onda()
    {
        emAnimacao = true;
        animator.Play(AnimacaoOnda, 0, 0f);

        Vector3 origem = transform.position + Vector3.up * 1f;
        CriarOnda(origem, transform.forward, Color.cyan);

        yield return new WaitForSeconds(tempoAnimacaoOnda * 0.35f);

        if (Physics.Raycast(origem, transform.forward, out RaycastHit hit, alcanceOnda))
        {
            CombatePersonagem1 alvo = hit.collider.GetComponentInParent<CombatePersonagem1>();
            if (alvo != null)
                alvo.ReceberHit();
        }

        yield return new WaitForSeconds(tempoAnimacaoOnda * 0.65f);
        emAnimacao = false;
    }

    public void ReceberHit()
    {
        if (!emAnimacao)
            StartCoroutine(TomarHit());
    }

    IEnumerator TomarHit()
    {
        emAnimacao = true;
        animator.Play(AnimacaoHit, 0, 0f);
        CriarEfeitoHit(Color.blue);
        yield return new WaitForSeconds(tempoAnimacaoHit);
        emAnimacao = false;
    }

    float DistanciaAte(Transform alvo)
    {
        return Vector3.Distance(transform.position, alvo.position);
    }

    void CriarEfeitoChute(Color cor)
    {
        GameObject efeito = GameObject.CreatePrimitive(PrimitiveType.Cube);
        efeito.transform.position = transform.position + transform.forward * 1.2f + Vector3.up * 0.5f;
        efeito.transform.localScale = new Vector3(0.5f, 0.3f, 0.5f);
        efeito.transform.rotation = transform.rotation;
        efeito.GetComponent<Renderer>().material.color = cor;
        Destroy(efeito.GetComponent<Collider>());
        Destroy(efeito, 0.35f);
    }

    void CriarOnda(Vector3 origem, Vector3 direcao, Color cor)
    {
        GameObject onda = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        onda.transform.position = origem + direcao * 1.5f;
        onda.transform.localScale = new Vector3(0.8f, 0.05f, 0.8f);
        onda.transform.rotation = Quaternion.LookRotation(direcao);
        onda.GetComponent<Renderer>().material.color = cor;
        Destroy(onda.GetComponent<Collider>());
        Destroy(onda, 0.5f);

        Debug.DrawRay(origem, direcao * alcanceOnda, cor, 0.5f);
    }

    void CriarEfeitoHit(Color cor)
    {
        GameObject efeito = GameObject.CreatePrimitive(PrimitiveType.Cube);
        efeito.transform.position = transform.position + Vector3.up * 1.2f;
        efeito.transform.localScale = Vector3.one * 0.5f;
        efeito.GetComponent<Renderer>().material.color = cor;
        Destroy(efeito.GetComponent<Collider>());
        Destroy(efeito, 0.4f);
    }
}
