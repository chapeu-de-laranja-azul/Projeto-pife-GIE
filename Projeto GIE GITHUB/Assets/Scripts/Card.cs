using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
	public bool hasBeenPlayed = false;  // atributo que controla se uma carta ja foi jogado ou n�o.
    public bool selecionado = false;    // atributo que controla se uma carta est� selecionada ou n�o. 
	public int handIndex;               // atributo que indica em qual posi��o na m�o do usu�rio est� a carta.
    public string valor, naipe, grupo;  // atributos da carta.
    public static float posz = 0;       // atributo est�tico utilizado para empilhar as cartas na pilha de descarte.
    public static bool select = false;  // atributo est�tico que controla se existe alguma carta na m�o selecionada ou n�o.
  
	GameManager gm;

	private Animator anim;
	private Animator camAnim;

	public GameObject effect;
	//public GameObject hollowCircle;

	private void Start()
	{
		gm = FindObjectOfType<GameManager>();
		anim = GetComponent<Animator>();
		camAnim = Camera.main.GetComponent<Animator>();
	}

    private void OnMouseDown()  //quando o usu�rio clicar em alguma carta...
	{
		if (!select && !hasBeenPlayed) // se n�o existe carta selecionada e a carta n�o est� na pilha de descarte...
		{
			SelecionaCarta();
		}
			
		else if (gm.mao.Count == 9 && this.hasBeenPlayed && this == gm.discardPile[gm.discardPile.Count-1]) // Verifica se o usu�rio pode pegar a �ltima carta da pilha de descarte.
        {
			PegaMorto();
		}	

        else if (select && !selecionado && !hasBeenPlayed) //Se ja existe alguma carta selecionada e o usu�rio clicou em outra que n�o est� na pilha de descarte, nesse caso ser� feita a troca das cartas na m�o.
        {
			TrocaCartas();
		}	

        else if(selecionado && gm.mao.Count == 10)   // Se a carta j� est� selecionada e na m�o do usu�rio tem 10 cartas, ent�o o usu�rio deseja descart�-la
		{
			Descartar();
		}	
        
        else if (!hasBeenPlayed)    // Por �ltimo, se a carta n�o est� na pilha de descarte, a �nica a��o � retorna-la para a posi��o original.
        {
            transform.position -= Vector3.up * 1f;  // retorna a carta para posi��o normal.
            selecionado = false;    				// indica que ela n�o est� selecioanda.
            select = false;         				// indica que n�o existem nenhuma carta selecionada.
        }
    }
	
	void SelecionaCarta()
	{
		transform.position += Vector3.up * 1f;      // coloca ela para cima quando clicar.
        selecionado = true;                         // indica que ela est� selecionada.
        select = true;                              // indica que existe uma carta selecionada.
	}

	void PegaMorto()
	{
		for (int i = 0; i < gm.availableCardSlots.Length; i++)  //percorro o meu array de slots para encontrar algum dispon�vel para colocar a carta que vem da pilha de descarte.
        {
            if (gm.availableCardSlots[i] == true)  //caso algum slot esteja dispon�vel...
            {
                this.handIndex = i;         // A posi��o da carta na m�o do jogador ser� igual ao slot dispon�vel.
                this.transform.position = gm.cardSlots[i].position;     // mudando a posi��o da carta (da pilha para o slot dispon�vel)
                this.hasBeenPlayed = false;      // colocando para false o atributo, indicando que a carta n�o est� mais na pilha de descarte.
                gm.discardPile.Remove(this);    // removo o carta da lista de cartas discardPile
                gm.availableCardSlots[i] = false;   // indico que o slot n�o est� mais dispon�vel
                camAnim.SetTrigger("shake");
                gm.mao.Insert(i,this);      // coloco a carta na lista de cartas m�o, na posi��o i.
                return;
            }      
        }
	}
	
	void TrocaCartas()
	{
		Vector3 slot1 = gm.cardSlots[this.handIndex].transform.position;    // guarda posi��o da carta que o usu�rio clicou
        Vector3 slot2 = slot1;      // cria uma segunda vari�vel para auxiliar na troca das posi��es.
        int idx1 = this.handIndex;  // vari�vel auxiliar que armazena o atributo handIndex da carta na m�o do usu�rio
        int idx2 = idx1;            // segunda vari�vel auxiliar para realizar a troca do atributo handIndex entre as duas cartas. 

        foreach (Card card in gm.mao)  //procura na m�o do usu�rio a outra carta j� selecionada
        {
            if (card.selecionado)   //se encontrar a carta selecionada...
            {
                slot2 = gm.cardSlots[card.handIndex].transform.position;    // armazena a posi��o da carta
                card.transform.position = slot1;  //realiza a troca de posica��o entre as duas cartas
                card.selecionado = false;   // indica que a carta n�o est� mais selecionada
                idx2 = card.handIndex;      // armazena o atributo handIndex da carta selecionada.
                card.handIndex = idx1;  // realiza a troca do atributo handIndex entre as duas cartas.
                break;
            }
        }
        
		this.transform.position = slot2;        // finaliza a troca de posi��o entre as cartas
        select = false;                         // indica que n�o existe mais nenhuma carta selecionada 
        this.selecionado = false;               // indica que a carta n�o est� mais selecionada
        this.handIndex = idx2;                  // realiza a troca do atributo handIndex entre as duas cartas.	
	}	
	
	void Descartar()
	{
		//Instantiate(hollowCircle, transform.position, Quaternion.identity);
			
		camAnim.SetTrigger("shake");  
		anim.SetTrigger("move");

        float posx = (float)Random.Range(-0.4f, 0.4f);      // posi��o x randomica 
        float posy = (float)Random.Range(-0.4f, 0.4f);      // posi��o y randomica
        posz -= 0.1f;                                       // posi��o z � decrementada em 0,1 para que a carta fique acima das demais na pilha de descarte.
        transform.position = new Vector3(posx, posy, posz); // muda a posi��o da carta que estava na m�o para uma posi��o randomica na pilha de descarte.
        hasBeenPlayed = true;                               // indica que a carta j� foi jogada, ou seja, est� na pilha de descarte.
		gm.availableCardSlots[handIndex] = true;            // indica que a posi��o que a carta ocupava agora est� dispon�vel.
        gm.mao.Remove(this); 								// remove a carta da lista de cartas m�o.
		
		Instantiate(effect, transform.position, Quaternion.identity);
        gm.discardPile.Add(this);   			 // adiciona a carta na lista de cartas discardPile
        select = false;             			 // indica que n�o existem cartas selecionadas
        selecionado = false;        			 // indica que a carta n�o est� selecionada.
	}	
}