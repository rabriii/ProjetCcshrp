using MySql.Data.MySqlClient;
using Dapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;
using Class1;

namespace Class1
{

    public class Patient 
    {
        public int code_patient { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
        public int age { get; set; }
        public Patient() 
        {
          
        }
    }
    
    public class Personnel 
    {
        public int code_perso { get; set; }
        public string nom { get; set; }
        public string prenom { get; set;}
        public string specialisation { get; set; }
        public Personnel()
        {
            
        }
    }

    public class Chambre 
    {
        public int num_chambre { get; set; }
        public int code_patient { get; set; }
        public DateTime date_entrer { get; set; }
        public DateTime date_sorti { get;set; }

        public Chambre()
        {
           
        }
    }

    public class Disponibilite 
    {
        public int code_perso { get; set; }
        public int code_patient { get; set; }
        public TimeSpan horaire { get; set; }

        public Disponibilite()
        {
            
        }
    }

    public class Connection
    {
        private string connectionString = "server=localhost;port=3306;database=gestion_hopital;uid=root;password=;";
        
        public Connection() { 
        MySqlConnection connection = new MySqlConnection(this.connectionString);

        }
        public void Creation(string tableName,object data ) 
        {
            using (var connection = new MySqlConnection(connectionString)) 
            {
                connection.Open();
                if (tableName == "disponibilite")
                {
                    connection.Execute($"INSERT INTO {tableName} VALUES(@arg1,@arg2,@arg3)", data);
                }
                else
                {
                    connection.Execute($"INSERT INTO {tableName} VALUES(@arg1,@arg2,@arg3,@arg4)", data);
                }
                
            }
        }

        public IEnumerable<T> Listage<T>(string tableName)
        {
            using (var connection = new MySqlConnection(connectionString)) 
            {
                connection.Open();
                return connection.Query<T>($"SELECT * FROM {tableName}");

            }
        }

        public IEnumerable<T> ListageSpecifique<T>(string tableName,string output)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                return connection.Query<T>($"SELECT {output} FROM {tableName}");

            }
        }

        public IEnumerable<T> ListageFiltrer<T>(string tableName,string colonne,string filtre)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                return connection.Query<T>($"SELECT * FROM {tableName} Where {colonne} = {filtre}");

            }
        }

        public IEnumerable<T> RechercherL<T>(string tableName,string dataName,string name)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                return connection.Query<T>($"SELECT * FROM {tableName} WHERE {dataName} LIKE '%{name}%'");

            }
        }
        public IEnumerable<T> Rechercher<T>(string tableName, string dataName, string name)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                return connection.Query<T>($"SELECT * FROM {tableName} WHERE {dataName} = {name}");

            }
        }

        public IEnumerable<T> RechercherParColonne<T>(string tableName, string colonne, string clePrimaire, string name)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                return connection.Query<T>($"SELECT {colonne} FROM {tableName} WHERE {clePrimaire} = {name}");

            }
        }

        //Recherche rehefa any @le manisy facture
        public IEnumerable<T> RechercherPatient<T>(int numPatient)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                return connection.Query<T>($"SELECT nom,prenom FROM patient WHERE code_patient = {numPatient}");

            }
        }
        public IEnumerable<T> RechercherDateSejour<T>(int numPatient)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                return connection.Query<T>($"SELECT date_entrer, date_sorti FROM chambre WHERE code_patient = {numPatient} ");

            }
        }

        public void Update(string tableName, object data,List<string> colonne, int id, string colonneId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                if (tableName == "disponibilite")
                {

                    connection.Execute($"UPDATE {tableName} SET {colonne[0]} = @arg1, {colonne[1]} = @arg2, {colonne[3]} = @arg3 WHERE {colonneId} = {id}", data);
                }
                else
                {
                    connection.Execute($"UPDATE {tableName} SET {colonne[0]} = @arg1, {colonne[1]} = @arg2, {colonne[2]} = @arg3, {colonne[3]} = @arg4 WHERE {colonneId} = {id}", data);
                }
                
            }
        }

        public void Delete(string tableName, int id, string colonneId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                connection.Execute($"DELETE FROM {tableName} WHERE {colonneId} = @Id", new { Id = id });
            }

        }



        }
    }

public class GestionErreur 
{
    public GestionErreur() 
    {

    }
    public bool nombreErreur(string nombre) 
    {
        if(int.TryParse(nombre, out int id)) 
        {
            
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool lettreErreur(string lettre)
    {
        if (int.TryParse(lettre,out int result))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}

public class Pdf
{
    private string outFile = Environment.CurrentDirectory + "/facture.pdf";
    
    public void GenererPdfUrgence(string titre, string nom, string prenom, int sommeMedicament, DateTime entrer, DateTime sorti,int prixSejour, int facture)
    {
        //Creation du document
        Document doc = new Document();
        PdfWriter.GetInstance(doc, new FileStream(outFile, FileMode.Create));
        doc.Open();

        //Couleurs
        BaseColor red = new BaseColor(224,17,12);
        BaseColor gris = new BaseColor(0, 0, 0);
        

        //Polices d'écriture
        Font policeTitre = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 20f, iTextSharp.text.Font.BOLD, red);
        Font policeTh = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, 1, gris);

        //Page
        //Creation des paragraphes
        Paragraph p1 = new Paragraph(titre+"\n\n\n",policeTitre);
        p1.Alignment = Element.ALIGN_CENTER;
        
        doc.Add(p1);

        Paragraph p2 = new Paragraph("Nom: "+nom + "\n\n", policeTh);
        p1.Alignment = Element.ALIGN_LEFT;
        doc.Add(p2);

        Paragraph p3 = new Paragraph("Prénom: " + prenom + "\n\n", policeTh);
        p1.Alignment = Element.ALIGN_LEFT;
        doc.Add(p3);

        Paragraph p4 = new Paragraph("Somme total des médicaments à payer : " + sommeMedicament + " Ariary" + "\n\n", policeTh);
        p1.Alignment = Element.ALIGN_LEFT;
        doc.Add(p4);

        Paragraph p5 = new Paragraph("Date d'entrer : " + entrer + " Ariary" + "\n\n", policeTh);
        p1.Alignment = Element.ALIGN_LEFT;
        doc.Add(p5);

        Paragraph p6 = new Paragraph("Date de sorti : " + sorti + " Ariary" + "\n\n", policeTh);
        p1.Alignment = Element.ALIGN_LEFT;
        doc.Add(p6);

        Paragraph p7 = new Paragraph("Prix de séjour passer dans l'hôpital : " + prixSejour + " Ariary" + "\n\n", policeTh);
        p1.Alignment = Element.ALIGN_LEFT;
        doc.Add(p7);

        Paragraph p8 = new Paragraph("Facture total à payer: " + facture +" Ariary" + "\n\n", policeTh);
        p1.Alignment = Element.ALIGN_LEFT;
        doc.Add(p8);

        //Fermer le document
        doc.Close();
        Process.Start(@"cmd.exe", @"/c" + outFile);
    }

    public void GenererPdfConsultation(string titre, string nom, string prenom, int sommeMedicament, int facture)
    {
        //Creation du document
        Document doc = new Document();
        PdfWriter.GetInstance(doc, new FileStream(outFile, FileMode.Create));
        doc.Open();

        //Couleurs
        BaseColor blue = new BaseColor(0, 25, 155);
        BaseColor gris = new BaseColor(0, 0, 0);
        

        //Polices d'écriture
        Font policeTitre = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 20f, iTextSharp.text.Font.BOLD, blue);
        Font policeTh = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, 1, gris);

        //Page
        //Creation des paragraphes
        Paragraph p1 = new Paragraph(titre + "\n\n\n", policeTitre);
        p1.Alignment = Element.ALIGN_CENTER;

        doc.Add(p1);

        Paragraph p2 = new Paragraph("Nom: " + nom + "\n\n", policeTh);
        p1.Alignment = Element.ALIGN_LEFT;
        doc.Add(p2);

        Paragraph p3 = new Paragraph("Prénom: " + prenom + "\n\n", policeTh);
        p1.Alignment = Element.ALIGN_LEFT;
        doc.Add(p3);

        Paragraph p4 = new Paragraph("Somme total des médicaments à payer : " + sommeMedicament + " Ariary" + "\n\n", policeTh);
        p1.Alignment = Element.ALIGN_LEFT;
        doc.Add(p4);

        Paragraph p5 = new Paragraph("Facture total à payer: " + facture + " Ariary" + "\n\n", policeTh);
        p1.Alignment = Element.ALIGN_LEFT;
        doc.Add(p5);

        //Fermer le document
        doc.Close();
        Process.Start(@"cmd.exe", @"/c" + outFile);
    }
}
