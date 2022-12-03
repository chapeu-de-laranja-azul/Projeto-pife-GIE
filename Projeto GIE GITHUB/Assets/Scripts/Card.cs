using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
	public bool hasBeenPlayed = false;  // atributo que controla se uma carta ja foi jogado ou não.
    public bool selecionado = false;    // atributo que controla se uma carta está selecionada ou não. 
	public int handIndex;               // atributo que indica em qual posição na mão do usuário está a carta.
    public string valor, naipe, grupo;  // atributos da carta.
    public static float posz = 0;       // atributo estático utilizado para empilhar as cartas na pilha de descarte.
    public static bool select = false;  // atributo estático que controla se existe alguma carta na mão selecionada ou não.
  
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

    private void OnMouseDown()  //quando o usuário clicar em alguma carta...
	{
		if (!select && !hasBeenPlayed) // se não existe carta selecionada e a carta não está na pilha de descarte...
		{
			SelecionaCarta();
		}
			
		else if (gm.mao.Count == 9 && this.hasBeenPlayed && this == gm.discardPile[gm.discardPile.Count-1]) // Verifica se o usuário pode pegar a última carta da pilha de descarte.
        {
			PegaMorto();
		}	

        else if (select && !selecionado && !hasBeenPlayed) //Se ja existe alguma carta selecionada e o usuário clicou em outra que não está na pilha de descarte, nesse caso será feita a troca das cartas na mão.
        {
			TrocaCartas();
		}	

        else if(selecionado && gm.mao.Count == 10)   // Se a carta já está selecionada e na mão do usuário tem 10 cartas, então o usuário deseja descartá-la
		{
			Descartar();
		}	
        
        else if (!hasBeenPlayed)    // Por último, se a carta não está na pilha de descarte, a única ação é retorna-la para a posição original.
        {
            transform.position -= Vector3.up * 1f;  // retorna a carta para posição normal.
            selecionado = false;    				// indica que ela não está selecioanda.
            select = false;         				// indica que não existem nenhuma carta selecionada.
        }
    }
	
	void SelecionaCarta()
	{
		transform.position += Vector3.up * 1f;      // coloca ela para cima quando clicar.
        selecionado = true;                         // indica que ela está selecionada.
        select = true;                              // indica que existe uma carta selecionada.
	}

	void PegaMorto()
	{
		for (int i = 0; i < gm.availableCardSlots.Length; i++)  //percorro o meu array de slots para encontrar algum disponível para colocar a carta que vem da pilha de descarte.
        {
            if (gm.availableCardSlots[i] == true)  //caso algum slot esteja disponível...
            {
                this.handIndex = i;         // A posição da carta na mão do jogador será igual ao slot disponível.
                this.transform.position = gm.cardSlots[i].position;     // mudando a posição da carta (da pilha para o slot disponível)
                this.hasBeenPlayed = false;      // colocando para false o atributo, indicando que a carta não está mais na pilha de descarte.
                gm.discardPile.Remove(this);    // removo o carta da lista de cartas discardPile
                gm.availableCardSlots[i] = false;   // indico que o slot não está mais disponível
                camAnim.SetTrigger("shake");
                gm.mao.Insert(i,this);      // coloco a carta na lista de cartas mão, na posição i.
                return;
            }      
        }
	}
	
	void TrocaCartas()
	{
		Vector3 slot1 = gm.cardSlots[this.handIndex].transform.position;    // guarda posição da carta que o usuário clicou
        Vector3 slot2 = slot1;      // cria uma segunda variável para auxiliar na troca das posições.
        int idx1 = this.handIndex;  // variável auxiliar que armazena o atributo handIndex da carta na mão do usuário
        int idx2 = idx1;            // segunda variável auxiliar para realizar a troca do atributo handIndex entre as duas cartas. 

        foreach (Card card in gm.mao)  //procura na mão do usuário a outra carta já selecionada
        {
            if (card.selecionado)   //se encontrar a carta selecionada...
            {
                slot2 = gm.cardSlots[card.handIndex].transform.position;    // armazena a posição da carta
                card.transform.position = slot1;  //realiza a troca de posicação entre as duas cartas
                card.selecionado = false;   // indica que a carta não está mais selecionada
                idx2 = card.handIndex;      // armazena o atributo handIndex da carta selecionada.
                card.handIndex = idx1;  // realiza a troca do atributo handIndex entre as duas cartas.
                break;
            }
        }
        
		this.transform.position = slot2;        // finaliza a troca de posição entre as cartas
        select = false;                         // indica que não existe mais nenhuma carta selecionada 
        this.selecionado = false;               // indica que a carta não está mais selecionada
        this.handIndex = idx2;                  // realiza a troca do atributo handIndex entre as duas cartas.	
	}	
	
	void Descartar()
	{
		//Instantiate(hollowCircle, transform.position, Quaternion.identity);
			
		camAnim.SetTrigger("shake");  
		anim.SetTrigger("move");

        float posx = (float)Random.Range(-0.4f, 0.4f);      // posição x randomica 
        float posy = (float)Random.Range(-0.4f, 0.4f);      // posição y randomica
        posz -= 0.1f;                                       // posição z é decrementada em 0,1 para que a carta fique acima das demais na pilha de descarte.
        transform.position = new Vector3(posx, posy, posz); // muda a posição da carta que estava na mão para uma posição randomica na pilha de descarte.
        hasBeenPlayed = true;                               // indica que a carta já foi jogada, ou seja, está na pilha de descarte.
		gm.availableCardSlots[handIndex] = true;            // indica que a posição que a carta ocupava agora está disponível.
        gm.mao.Remove(this); 								// remove a carta da lista de cartas mão.
		
		Instantiate(effect, transform.position, Quaternion.identity);
        gm.discardPile.Add(this);   			 // adiciona a carta na lista de cartas discardPile
        select = false;             			 // indica que não existem cartas selecionadas
        selecionado = false;        			 // indica que a carta não está selecionada.
	}	
}