using HtmlAgilityPack;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using System.Threading.Tasks;
using System.Drawing;
using WordCloud;
using System.Text;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        //Conectarse al sitio web
        string url = "https://cienciasdelsur.com/";
        WebClient client = new WebClient();
        string html = client.DownloadString(url);
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);
        List<string> enlaces = new List<string>(); // creo una lista para guardar todos los enlaces del foro

        // Encuentro todos los elementos <a> de dicha clase
        foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[contains(@class, 'td-image-wrap')]"))
        {
            enlaces.Add(link.Attributes["href"].Value); //guardo los enlaces en una lista
        }

        string texto = ""; // en esta variable voy a guardar todo el documento que ira exportado a un archivo
        int i = 4; // el contador empieza en 4, porque ahi esta el primer enlace

        using (StreamWriter writer = new StreamWriter("EXTRACCION_TEXTOS.txt", false)) //se crea el archivo
        {
            writer.Write(texto);
        }

        while (i < enlaces.Count) // con este while recorre la lista de enlaces para scrapear todos los enlaces
        {
            string url2 = enlaces[i];                
            WebClient client2 = new WebClient();
            string html2 = client2.DownloadString(url2);
            HtmlDocument doc2 = new HtmlDocument();
            doc2.LoadHtml(html2);
            string titulo = doc2.DocumentNode.SelectSingleNode("//h1[@class='entry-title']").InnerText; // se extrae el titulo del articulo
            texto += titulo; // se guarda el titulo para luego concatenar con el articulo
            HtmlNode articulo = doc2.DocumentNode.SelectSingleNode("//div[@class='td-post-content tagdiv-type']"); // extraigo todo el contenido del articulo
            texto += articulo.InnerText; //se guarda el articulo
            using (StreamWriter f = new StreamWriter("EXTRACCION_TEXTOS.txt", true, System.Text.Encoding.UTF8)) //se escribe en el archivo los articulos
            {
                f.Write(texto);
            }
           i++; // paso al siguiente enlace
           texto = ""; //reseteo la variable para el siguente enlace
        }

        // Lista de palabras a excluir
        List<string> palabrasExcluir = new List<string>() { "yo", "tu", "él", "ella", "nosotros", "vosotros", "ellos", "ellas","de","la","y","en",
        "que","el","a","del","las","los","se","para","es","un","una","por","con","no","más","como","al","o","su","El","author","com","https","son",
        "este","nbsp","cienciasdelsur","lo","sobre","también","sus","e","puede","ser","esta","desde","entre","qué","fue","alejandra","foto","uno",
        "si","5","Qué","está","debe","parte","otros","pero","forma","sin","ha","ejemplo","nos","te","tiene","sosa","tienen","cómo","muy","través",
        "todo","gran","donde","ya","hay","falta","4","vez","así","además","the","sino","benítez","hasta","estas","1","about","esto","datos","sea",
        "esto","le","cual","tener","min","posts","related","compartir","cuando","acuerdo","uso","tenemos","p","hill","graw","mc","ed","pequeños",
        "pueden","luego","glkuwssnkcrhomwm","aine","alvarenga","adriana","algún","tal","casos","presenta","cada","otras","porque","estos","personas",
        "otra","dos","actualmente","acceso","ni","según","trabajo"};

        string contenido = File.ReadAllText("EXTRACCION_TEXTOS.txt",System.Text.Encoding.UTF8); // Leo el archivo

        // Divide la cadena de texto de acuerdo a la siguente expresion regular y las convierte a minuscula
        Regex rgx = new Regex(@"[^\p{L}\p{N}]+");
        string[] palabras = Regex.Split(contenido.ToLower(), @"\W+");

        Dictionary<string, int> frecuencias = new Dictionary<string, int>(); //diccionario para guardar las palabras con sus apariciones
        //en este ciclo se carga el diccionario con las palabras y sus apariciones
        foreach (string palabra in palabras)
        {
            if (!frecuencias.ContainsKey(palabra) && !palabrasExcluir.Contains(palabra)) 
            {
                frecuencias[palabra] = 0;
            }
            if (!palabrasExcluir.Contains(palabra))
            {
                frecuencias[palabra]++;
            }
        }

        var palabrasOrdenadas = frecuencias.OrderByDescending(x => x.Value); // Ordena el diccionario por valor de frecuencia en orden descendente
        int contador = 0; //contador para evaluar las 50 primeras
        // Muestra las primeras 50 palabras del diccionario ordenado
        foreach (var palabra in palabrasOrdenadas)
        {
            Console.WriteLine("{0}: {1}", palabra.Key, palabra.Value);
            contador++;
            if (contador == 50) break;
        }
        // Creo una cadena de texto con las palabras y sus frecuencias
        StringBuilder sb = new StringBuilder();
        
        //listas para el wordcloud
        List<string> claves = new List<string>();;
        List<int> valores = new List<int>();;

        foreach (var palabra in palabrasOrdenadas.Take(50))
        {
            sb.AppendLine($"{palabra.Key}: {palabra.Value}");
            claves.Add(palabra.Key);   //carga las palabras para usar en el grafico
            valores.Add(palabra.Value);//carga las frecuencias para usar en el grafic
        }
        //se guardan las palabras y sus frecuencias en un nuevo archivo
        using (StreamWriter f = new StreamWriter("FRECUENCIA_PALABRAS.txt", false, System.Text.Encoding.UTF8))
        {
            f.Write(sb.ToString());
        }
        Color? fontColor = Color.White;
        WordCloud.WordCloud wc = new WordCloud.WordCloud(800, 600, true);
        wc.Draw(claves, valores).Save(@".\WordCloud.JPG");
        //La imagen se guarda en el directorio


        //Item 2
        //Conectarse al sitio web
        url = "https://cienciasdelsur.com/";
        client = new WebClient();
        html = client.DownloadString(url);
        doc = new HtmlDocument();
        doc.LoadHtml(html);
        enlaces = new List<string>(); // creo una lista para guardar todos los enlaces del foro

        // Encuentro todos los elementos <a> de dicha clase
        foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[contains(@class, 'td-image-wrap')]"))
        {
            enlaces.Add(link.Attributes["href"].Value); //guardo los enlaces en una lista
        }

        i = 4; // el contador empieza en 4, porque ahi esta el primer enlace
        Console.Write("Ingrese su palabra compuesta: ");// Solicitar palabra compuesta
        var palabra_compuesta = Console.ReadLine();
        var apariciones = 0; // variable para almacenar todas las apariciones de la palabra compuesta   
        while (i < enlaces.Count) // este while es basicamente igual al primero
        {
            string url2 = enlaces[i];                
            WebClient client2 = new WebClient();
            string html2 = client2.DownloadString(url2);
            HtmlDocument doc2 = new HtmlDocument();
            doc2.LoadHtml(html2);
            var text = doc2.DocumentNode.SelectSingleNode("//body").InnerText;// Extraigo el texto de toda la página
            // Buscar la palabra compuesta en el texto de la página
            if (text.Contains(palabra_compuesta))
            {
                apariciones++;
            }
            
           i++; // paso al siguiente enlace
        }

        // Mostrar todas las apariciones encontradas de la palabra compuesta
        if (apariciones > 0)
        {
            Console.WriteLine($"Se encontraron {apariciones} apariciones de la palabra compuesta: {palabra_compuesta}");
        }
        else
        {
            Console.WriteLine("No se encontraron apariciones de la palabra compuesta.");
        }
    }
}

 