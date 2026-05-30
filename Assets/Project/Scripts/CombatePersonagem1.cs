using System.Collections;
using UnityEngine;

/// <summary>
/// Combate do Personagem 1.
/// Botão esquerdo = soco (corpo a corpo) | Botão direito = tiro (distância)
/// Animações: Jump (soco), Fast Run (tiro), Crouched Idle (tomar hit)
/// </summary>
public class CombatePersonagem1 : MonoBehaviour
{
    const string AnimacaoSoco = "Jump";
    const string AnimacaoTiro = "Fast Run";
    const string AnimacaoHit = "Crouched Idle";

    public float alcanceSoco = 2f;
    public float alcanceTiro = 15f;
    public float tempoAnimacaoSoco = 0.6f;
    public float tempoAnimacaoTiro = 0.5f;
    public float tempoAnimacaoHit = 0.8f;

    CombatePersonagem2 oponente;
    Animator animator;
    bool ativo;
    bool emAnimacao;

    public bool EstaEmAnimacao => emAnimacao;

    public void Configurar(CombatePersonagem2 oponenteCombate)
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
            StartCoroutine(Soco());

        if (Input.GetMouseButtonDown(1))
            StartCoroutine(Tiro());
    }

    IEnumerator Soco()
    {
        emAnimacao = true;
        animator.Play(AnimacaoSoco, 0, 0f);

        CriarEfeitoSoco(Color.yellow);

        yield return new WaitForSeconds(tempoAnimacaoSoco * 0.4f);

        if (oponente != null && DistanciaAte(oponente.transform) <= alcanceSoco)
            oponente.ReceberHit();

        yield return new WaitForSeconds(tempoAnimacaoSoco * 0.6f);
        emAnimacao = false;
    }

    IEnumerator Tiro()
    {
        emAnimacao = true;
        animator.Play(AnimacaoTiro, 0, 0f);

        Vector3 origem = transform.position + Vector3.up * 1.2f;
        Vector3 direcao = transform.forward;
        CriarProjetil(origem, direcao, Color.red);

        yield return new WaitForSeconds(tempoAnimacaoTiro * 0.3f);

        if (Physics.Raycast(origem, direcao, out RaycastHit hit, alcanceTiro))
        {
            CombatePersonagem2 alvo = hit.collider.GetComponentInParent<CombatePersonagem2>();
            if (alvo != null)
                alvo.ReceberHit();
        }

        yield return new WaitForSeconds(tempoAnimacaoTiro * 0.7f);
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
        CriarEfeitoHit(Color.red);
        yield return new WaitForSeconds(tempoAnimacaoHit);
        emAnimacao = false;
    }

    float DistanciaAte(Transform alvo)
    {
        return Vector3.Distance(transform.position, alvo.position);
    }

    void CriarEfeitoSoco(Color cor)
    {
        GameObject efeito = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        efeito.transform.position = transform.position + transform.forward * 1f + Vector3.up;
        efeito.transform.localScale = Vector3.one * 0.4f;
        efeito.GetComponent<Renderer>().material.color = cor;
        Destroy(efeito.GetComponent<Collider>());
        Destroy(efeito, 0.3f);
    }

    void CriarProjetil(Vector3 origem, Vector3 direcao, Color cor)
    {
        GameObject projetil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projetil.transform.position = origem;
        projetil.transform.localScale = Vector3.one * 0.2f;
        projetil.GetComponent<Renderer>().material.color = cor;
        Destroy(projetil.GetComponent<Collider>());
        Destroy(projetil, 0.4f);

        Debug.DrawRay(origem, direcao * alcanceTiro, cor, 0.5f);
    }

    void CriarEfeitoHit(Color cor)
    {
        GameObject efeito = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        efeito.transform.position = transform.position + Vector3.up * 1.2f;
        efeito.transform.localScale = Vector3.one * 0.6f;
        efeito.GetComponent<Renderer>().material.color = cor;
        Destroy(efeito.GetComponent<Collider>());
        Destroy(efeito, 0.4f);
    }
}
