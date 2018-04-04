using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Humanitas.Services.Interfaces;

namespace Humanitas.Services.Tests
{
    [TestClass]
    public class TerminalServiceTests
    {
        [TestMethod]
        public void Run_HappyPath()
        {
            ITerminalService service = new TerminalService(TestHelper.GetAppConfiguration(), new UserAccessService(TestHelper.GetAppConfiguration()));
            var output = service.Run($@"Autor: Henry Cobin
Tópico: Predestinação
Tópico: Personalidade Islâmica
Tópico: Direito de um é a obrigação do outro
Tópico: Dois e dois são quatro, onde está minha vontade nisso?
Tópico: O dever ser é a morte de toda vida moral
Livro: Quarup [Antônio Callado]
Autor: Carlos Heitor Cony
Livro: Pessach: a travessia [Carlos Heitor Cony]
Autor: Lêdo Ivo
Livro: A morte do Brasil [Lêdo Ivo]
Autor: Leonel Brizola
Autor: Joseph Conrad
Livro: O Exército Na Historia Do Brasil [Olavo De Carvalho]
Livro: 1968: o Ano Que não Terminou [Zuenir Ventura]
Autor: William Faulkner
Autor: Henry James
Autor: Frank Norris
Livro: McTeague [Frank Norris]
Livro: Os Caminhos da Liberdade [Jean-Paul Sartre]
Autor: André Gide
Autor: Julien Green
Autor: Etienne Souriau
Livro: As duzentas mil situações dramáticas [Etienne Souriau]
Autor: Edwin Muir
Livro: The Structure Of The Novel [Edwin Muir]
Autor: Edward Morgan Forster
Livro: Aspects of the Novel [Edward Morgan Forster]
Autor: Alexander Sutherland Neill
Livro: Liberdade sem medo [Alexander Sutherland Neill]
Livro: A túnica e os dados [José Geraldo Vieira]
Livro: A Ladeira da Memória [José Geraldo Vieira]

""No Brasil a maledicência é prova de nobreza.""
Olavo de Carvalho

""A classe média limpinha e arrumadinha exige muito em troca de uma aprovação que, no fim das contas, não significa nada.""
Olavo de Carvalho

""O egoísmo é próprio de um eu vacilante, que se sente vazio e fraco sem uma constante renovação do seu estoque de complementos.""
Olavo de Carvalho

""Se Jesus exigisse fidelidade incondicional, teria demitido S. Pedro.""
Olavo de Carvalho

""A oratória agrada aos ouvidos, mas só a fidelidade das palavras às impressões genuínas toca os corações.""
Olavo de Carvalho

""A pungência do discurso não depende da ênfase oratória, mas da clareza com que você viu o seu objeto e da busca da expressão certa.""
Olavo de Carvalho

""A antiga retórica dividia os estilos em solene, médio e simples. Planejo guardar o primeiro só para orações fúnebres de santos, heróis e mártires.""
Olavo de Carvalho

""O amor morre porque o condicionamos a mil exigências que ele não pode cumprir: conforto, utilidade, funcionalidade social, prosperidade, até mesmo a fidelidade -- tudo conspira para matá-lo.""
Olavo de Carvalho

""O amor é falso enquanto não se torna tão incondicional e singelo quanto o apego do bebê pela sua mãe e da mãe pelo seu bebê.""
Olavo de Carvalho

""Nossas fantasias -- de sexo, de dinheiro, de poder, de sucesso -- reconstroem instantaneamente o nosso eu em torno de um ponto qualquer da periferia que atraiu a nossa atenção e que naquele momento nos parece o centro de tudo. Nosso verdadeiro centro está sempre num ponto intermediário entre a ilusão e a decepção.""
Olavo de Carvalho

""O desprezo pelo 'eu' é a marca registrada da pseudomística. O eu é a coisa mais preciosa que Deus nos deu, porque é a única que se parece com Ele e é ela que vai ser julgada no Juízo Final.""
Olavo de Carvalho

""Que Deus nos preserve de escrever uma só palavra que antes não tenhamos dito em silêncio, diante d'Ele, com temor e tremor.""
Olavo de Carvalho

""Usar o Nome de Deus em vão é quase a definição da oratória religiosa.""
Olavo de Carvalho

""Se o que você diz sobre Deus não doeu, não deveria ter sido dito.""
Olavo de Carvalho

""O discurso da fé é sempre incompleto, semanticamente, porque aí não podemos ter o domínio cognitivo satisfatório dos objetos a que nos referimos. Eis por que nessa área é tão tentador cair no verbalismo oco, nos chavões e na busca de efeitos fáceis. A única substância que podemos dar a esse discurso é a da nossa própria busca de Deus, a dos nossos próprios tormentos interiores, a da nossa própria insuficiência, esperando que Ele mesmo complete, de algum modo, um sentido que nos escapa.""
Olavo de Carvalho

""As histórias do Comissário Maigret são tão repletas de sentido humano quanto os outros romances de Georges Simenon, autor que completava um livro em onze dias contados. Nem entendo como alguém pode enxergar a vida por tantos ângulos diversos e descrever tudo com tamanha naturalidade. Simenon foi um dos grandes escritores do século XX.""
Olavo de Carvalho

""Em 1966 aparecia o estudo clássico de Richard Hofstadter, 'Anti-Intellectualism in American Life'. No Brasil, onde é muito mais intenso o ódio à vida intelectual (acompanhado do amor sem fim aos seus símbolos exteriores), até hoje não se fez um estudo similar, o que prova que, quanto mais grave é a doença, maior o desejo se ocultá-la.""
Olavo de Carvalho

""O método brasileiro de fazer biografias é jamais perguntar nada ao biografado.""
Olavo de Carvalho

""A compreensão é o nível mais baixo, mais imediato e mais indispensável do amor ao próximo. Daí aquilo que o Lionel Trilling chamava 'o dever moral de ser inteligente'.""
Olavo de Carvalho

""Todo dia confesso a Deus que a minha fé é uma bela porcaria, e pergunto a Ele como pode uma teologia cristã erguer-se em cima de uma cosmologia materialista, que é a única que temos hoje em dia. Ele nunca me respondeu, o que para mim tem um sentido muito claro:
-- Não pode.""
Olavo de Carvalho

""A falha humana que eu mais gostaria de corrigir é a agressividade. Ela pode ter sido uma vantagem na época dos homens das cavernas, para que eles pudessem obter mais comida, território ou uma parceira com quem se reproduzir, mas, agora, ela ameaça destruir todos nós.""
Stephen Hawking

""Ninguém quer o que não se conhece.""
Rafael Melo

""O encantamento intelectual e de caráter consiste em responder às perguntas que o sujeito está fazendo.""
Rafael Melo

""No Brasil, a religião se resume a duas coisas: teologia da prosperidade e simpatias medicinais. A primeira auxilia-nos a ganhar algum dinheirinho ou promoção no trabalho; a segunda a curar as hemorroidas de nossos cus. E isso se aplica tanto aos terreiros de umbanda quanto às igrejas evangélicas. Com relação ao catolicismo, não há nem o que falar: nasceu de parto prematuro e está em coma até hoje.""
Rafael Melo

""Há autores ilustres que, por mais que eu tente, não consigo ler além de uma dúzia de páginas. Cormac McCarthy, Thomas Pynchon, Michel de Montaigne e Charles Maurras são exemplos. Nada tenho contra eles, mas, como diz o povo, 'não dá liga'.
Acho que isso acontece com todo mundo, mas logo o pessoal diz 'Não gostei' antes mesmo de ler o suficiente para saber se gosta ou não gosta.""
Olavo de Carvalho

""Considero um ato de subserviência intelectual à modernidade a tentativa de C. S. Lewis de revalorizar esteticamente a simbólica medieval sem nem por um instante admitir que houvesse nela algo mais que uma beleza desprovida de verdade. V. C. S. Lewis, 'The Discarded Image: An In-troduction to Medieval and Renaissance Literature', Cambridge University Press, 2012 (reprint). Não duvido de que, quando ele chamou René Guénon de “charlatão”, houvesse nisso uma ponta de despeito pela compreensão superior que o escritor francês tinha do simbolismo. Guénon foi um agente do imperialismo islâmico, mas nunca um charlatão.""
Olavo de Carvalho

""Nunca fui presunçoso o bastante para achar que meus sentimentos pessoais devem governar a política. Tenho mais simpatia pelas putas, mendigos e bêbados do que pela burguesia, mas isso não quer dizer que eles devam governar o mundo.""
Olavo de Carvalho

""A coisa mais maravilhosa de uma vida de escritor é que sua profissão consiste apenas em você ser você mesmo.""
Olavo de Carvalho

""Em defesa da minha honra, declaro que na juventude frequentei muitos puteiros, mas nenhuma universidade.""
Olavo de Carvalho

""Em geral as pessoas só reagem -- externa e internamente -- a fatos que são do domínio comum do seu meio social mediato e imediato. O homem que investiga por si mesmo, tendo acesso a fontes mais ricas e fidedignas do que a mídia popular e o zunzum do quarteirão, terá portanto reações e decisões que parecerão estranhas, incompreensíveis ou chocantes à platéia em torno, a qual provavelmente só virá a compreendê-las depois de muitas décadas. Não é sensato você querer saber o que os outros não sabem e esperar que eles o compreendam na mesma hora. A incapacidade ou relutância de suportar essa tensão é uma das maiores causas de fracasso numa vida de estudos. 
Os intelectuais de esquerda gostam de posar de 'outsiders', mas na verdade só sabem ser outsiders coletivos e sob a proteção do 'establishment'.""
Olavo de Carvalho

""Opor-se a um discurso ideológico, sem fazer nada contra o esquema político concreto que se utiliza dele, é o mesmo que discursar contra o roubo para não ter de prender os ladrões.""
Olavo de Carvalho

""A suprema covardia é o medo de ter medo.""
Olavo de Carvalho

""O clero [brasileiro] divide-se entre comunistas triunfantes e isentões acovardados, subservientes ao diabo de batina.""
Olavo de Carvalho

""A ditadura dos megabilionários parece inevitável -- com o apoio entusiástico da militância esquerdista.""
Olavo de Carvalho

""O Brasil está repleto de cópias da vovozinha do conto da Flannery O'Connor, 'A good man is hard to find', que prefere antes deixar-se matar do que admitir que os assassinos são assassinos.""
Olavo de Carvalho

""Pergunta horrível e necessária: Há ou não um certo prazer em renunciar a certos prazeres pelo prazer de proibi-los aos outros?""
Olavo de Carvalho

""A maior prova de mentalidade mesquinha é a pressa em sentir-se ofendido.""
Olavo de Carvalho", "EAAE6keP0ZCAABAIEVkAOc0C6NhkZAUA4WDVkfoNkzZCSoBV2ZCjXZBMIpjwz0ytCveodG0Eh98m5l4OHPVDLZAWZAl6YidNsW2XkIGdZACz0ioLvX9IpcSidJbp4FRlZCU0B46KsZCtbUtspmFd5VGZBHyec3cjtKozD4mLKqvdYTGkK8mGYllZCSZAaQhzkwnP4EDkjxWTGr2HzWMAZDZD");

            output.ToString();
        }
    }
}
