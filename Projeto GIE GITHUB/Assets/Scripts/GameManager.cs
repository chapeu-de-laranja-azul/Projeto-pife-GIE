using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

	public List<Card> deck;  //lista de cartas que representa a baralho.
	public TextMeshProUGUI deckSizeText;    //texto que apresenta na tela o n�mero de cartas no baralho.

	public Transform[] cardSlots;   // transform que representa os slots das cartas.
    public bool[] availableCardSlots;   // array que controla os slots livres e ocupados. 

	public List<Card> discardPile;    // lista de cartas que representa a lista de descarte.
	public TextMeshProUGUI discardPileSizeText;  // texto que representa na tela o n�mero de cartas na pilha de descarte.

    public List<Card> mao;  // lista de cartas que representa as cartas na m�o do usu�rio.
    public List<Card> jogos;  // lista de cartas que representa os jogos do usu�rio.

    public int contJogos = 0;   // atributo utilizado para contagem de jogos fechados do usu�rio.

    private Animator camAnim;

	private void Start()
	{
        
        for (int i = 0; i < cardSlots.Length-1; i++)  // come�a o jogo com as 9 cartas do usu�rio na m�o.
        {
            mao.Add(deck[Random.Range(0, deck.Count)]);   // adiciona para a m�o uma carta aleat�ria do baralho.
            mao[i].gameObject.SetActive(true);            // ativa o objeto para ser mostrado em tela.
            mao[i].handIndex = i;                         // indica a posi��o na m�o que a carta est� 
            mao[i].transform.position = cardSlots[i].position; // coloca a carta na posi��o correta de acordo com o slot.
            mao[i].hasBeenPlayed = false;                  // indica que a carta n�o foi jogada.
            deck.Remove(mao[i]);                           // remove a carta do baralho.
            availableCardSlots[i] = false;                 // indica que o slot est� ocupado agora.
        }
        camAnim = Camera.main.GetComponent<Animator>();
    }

	public void DrawCard() //fun��o para comprar cartas
	{
        if (deck.Count >= 1 && mao.Count == 9)  //se no baralho existe ao menos 1 carta e na m�o o usu�rio possui 9 cartas...
		{
			camAnim.SetTrigger("shake");

			for (int i = 0; i < availableCardSlots.Length; i++)   //procura na m�o do usu�rio o slot livre para colocar a nova carta.
			{
				if (availableCardSlots[i] == true)      //se encontrar o slot dispon�vel...
				{
                    mao.Insert(i,deck[Random.Range(0, deck.Count)]);  //insere na m�o do usu�rio, na posi��o livre, uma carta aleat�ria do baralho.  
                    mao[i].gameObject.SetActive(true);  // ativa o objeto para ser mostrada em tela.
                    mao[i].handIndex = i;               // indica a posi��o na m�o que a carta est�.
                    mao[i].transform.position = cardSlots[i].position; // coloca a carta na posi��o correta de acordo com o slot dispon�vel.
                    mao[i].hasBeenPlayed = false;  // indica que a carta n�o foi jogada.
					deck.Remove(mao[i]);           // remove a carta do baralho. 
					availableCardSlots[i] = false; // indica que o slot est� ocupado agora.
                    return;
				}
			}
		}
	}

    public void Shuffles() // fun��o para pegar as cartas da pilha de descarte e juntar ao baralho.
	{
		if (discardPile.Count >= 1) //se existir alguma carta na pilha de descarte...
		{
			foreach (Card card in discardPile)  // para cada carta na pilha de descarte...
			{
                card.gameObject.SetActive(false);   // desativa o objeto para sumir da tela.
                card.hasBeenPlayed = false;         // indica que a carta n�o foi jogada. 
				deck.Add(card);                     // adiciona a carta ao baralho.
			}
			discardPile.Clear();                    // limpa a pilha de descarte.
		}
	}

	private void Update()
	{
		deckSizeText.text = deck.Count.ToString();   //atualiza o texto contador de cartas no baralho.
		discardPileSizeText.text = discardPile.Count.ToString();  //atualiza o texto contador de cartas na pilha de descarte.
    }

    public void Shuffle() //fun��o que verifica se o usu�rio bateu.
    {
        if (discardPile.Count >= 1 && deck.Count == 0)
        {
            foreach (Card card in discardPile)
            {
                card.gameObject.SetActive(false);
                card.hasBeenPlayed = false;
                deck.Add(card);
            }
            discardPile.Clear();
        }

        else if (mao.Count == 10) // se o usu�rio possui 10 cartas na m�o...
        {
            contJogos = 0;  // contador de jogos fechados � zerado.
            jogos.Clear();  // a lista de jogos � limpa.

            for(int i = 0; i < 9; i++) 
                jogos.Add(mao.Find(x => x.handIndex == i));  //preenche a lista de carta jogos com as cartas da m�o do usu�rio de acordo com a posi��o das cartas (handIndex)

            VerificaTrinca();   // fun��o que verifica se o usu�rio possui trincas fechadas.
            VerificaSeq();      // fun��o que verifica se o usu�rio possui sequ�ncias fechadas.

            Debug.Log(contJogos);

            if (contJogos == 3)     //se o usu�rio possui 3 jogos fechados...
                Debug.Log("Bateu!");   
            else
                Debug.Log("N�o bateu!");
        }
    }

    public void VerificaTrinca() //fun��o que verifica se o usu�rio possui trincas fechadas.
    {
        if (jogos[0].valor == jogos[1].valor && jogos[0].valor == jogos[2].valor) //verifica se nas posi��es 0, 1 e 2 o valor das cartas � o mesmo. Se for o usu�rio fechou um jogo 
            contJogos++;

        if (jogos[3].valor == jogos[4].valor && jogos[3].valor == jogos[5].valor) //verifica se nas posi��es 3, 4 e 5 o valor das cartas � o mesmo. Se for o usu�rio fechou um jogo
            contJogos++;

        if (jogos[6].valor == jogos[7].valor && jogos[6].valor == jogos[8].valor) //verifica se nas posi��es 6, 7 e 8 o valor das cartas � o mesmo. Se for o usu�rio fechou um jogo
            contJogos++;
    }

    public void VerificaSeq() //fun��o que verifica se o usu�rio possui sequ�ncias fechadas.
    {
        if (jogos[0].naipe == jogos[1].naipe && jogos[0].naipe == jogos[2].naipe) //verifica se nas posi��es 0, 1 e 2 o naipe das cartas � o mesmo. 
        {
            if(jogos[0].grupo == jogos[1].grupo && jogos[0].grupo == jogos[2].grupo) //verifica se nas posi��es 0, 1 e 2 o grupo das cartas � o mesmo.
                contJogos++;                                                         //se for o usu�rio fechou um jogo de sequ�ncia.
            else if (jogos[0].grupo == "coringa")                   //verifica se na posi��o 0 cont�m um coringa...
                if (jogos[1].grupo == jogos[2].grupo)               // se tiver, verifica se nas posi��es 1 e 2 o grupo das cartas � o mesmo.
                    contJogos++;                                    // se for o usu�rio fechou um jogo de sequ�ncia com coringa.
            else if (jogos[1].grupo == "coringa")                   
                if (jogos[0].grupo == jogos[2].grupo)
                    contJogos++;
            else if (jogos[2].grupo == "coringa")
                if (jogos[0].grupo == jogos[1].grupo)
                    contJogos++;
        }

        if (jogos[3].naipe == jogos[4].naipe && jogos[3].naipe == jogos[5].naipe) //verifica se nas posi��es 3, 4 e 5 o naipe das cartas � o mesmo.
        {
            if (jogos[3].grupo == jogos[4].grupo && jogos[3].grupo == jogos[5].grupo)   //verifica se nas posi��es 3, 4 e 5 o grupo das cartas � o mesmo.
                contJogos++;                                                            //se for o usu�rio fechou um jogo de sequ�ncia.
            else if (jogos[3].grupo == "coringa")                   //verifica se na posi��o 3 cont�m um coringa...
                if (jogos[4].grupo == jogos[5].grupo)               // se tiver, verifica se nas posi��es 4 e 5 o grupo das cartas � o mesmo.
                    contJogos++;                                    // se for o usu�rio fechou um jogo de sequ�ncia com coringa.
            else if (jogos[4].grupo == "coringa")                   
                if (jogos[3].grupo == jogos[5].grupo)
                    contJogos++;
            else if (jogos[5].grupo == "coringa")
                if (jogos[3].grupo == jogos[4].grupo)
                    contJogos++;
        }

        if (jogos[6].naipe == jogos[7].naipe && jogos[6].naipe == jogos[8].naipe) //verifica se nas posi��es 0, 1 e 2 o naipe das cartas � o mesmo.
        {
            if (jogos[6].grupo == jogos[7].grupo && jogos[6].grupo == jogos[8].grupo)    //verifica se nas posi��es 6, 7 e 8 o grupo das cartas � o mesmo.
                contJogos++;                                                             //se for o usu�rio fechou um jogo de sequ�ncia.
            else if (jogos[6].grupo == "coringa")                   //verifica se na posi��o 6 cont�m um coringa...
                if (jogos[7].grupo == jogos[8].grupo)               // se tiver, verifica se nas posi��es 7 e 8 o grupo das cartas � o mesmo.
                    contJogos++;                                    // se for o usu�rio fechou um jogo de sequ�ncia com coringa.
            else if (jogos[7].grupo == "coringa")                   
                if (jogos[6].grupo == jogos[8].grupo)
                    contJogos++;
            else if (jogos[8].grupo == "coringa")
                if (jogos[6].grupo == jogos[7].grupo)
                    contJogos++;
        }
    }
}
