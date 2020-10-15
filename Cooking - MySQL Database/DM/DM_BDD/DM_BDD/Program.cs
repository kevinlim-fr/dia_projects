using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;
using System.Xml;

namespace DM_BDD
{
    public class Program
    {
        /// <summary>
        /// Classe Client
        /// </summary>
        public class Client
        {
            //Attributs
            private string client_nom;
            private string client_tel;
            private string client_adresse;
            private bool client_createur;
            private float client_solde = 0;
            /// <summary>
            /// Constructeur permettant de récupérer les informations du client depuis la base de donnée SQL
            /// </summary>
            /// <param name="client_nom">Nom du client à rechercher dans la base de donnée</param>
            /// <param name="co">permet d'effectuer la connection et de lancer la requette</param>
            public Client(string client_nom, CoSQL co) //On l'utilise pour créer un objet C# client qui est dans la base de données SQL
            {
                this.client_nom = client_nom;
                this.client_tel = co.Requete_lecture("SELECT tel_client FROM client WHERE nom_client = '" + client_nom + "'");
                this.client_adresse = co.Requete_lecture("SELECT adresse_client FROM client WHERE nom_client = '" + client_nom + "'");
                if (co.Requete_lecture("SELECT nom_cdr FROM client WHERE nom_client = '" + client_nom + "'") == client_nom)
                {
                    this.client_createur = true;
                    this.client_solde = float.Parse(co.Requete_lecture("SELECT solde_cdr FROM createur_de_recettes WHERE nom_cdr = '" + client_nom + "'"));
                }
            }
            /// <summary>
            /// Constructeur permettant de créer un nouveau client depuis C# et d'insérer ces informations dans la base de donnée
            /// </summary>
            /// <param name="client_nom"></param>
            /// <param name="client_tel"></param>
            /// <param name="client_adresse"></param>
            /// <param name="client_createur"></param>
            /// <param name="co"></param>
            public Client(string client_nom, string client_tel, string client_adresse, bool client_createur, CoSQL co)  //On l'utilise pour créer un client qui n'existe pas encore
            {
                this.client_nom = client_nom;
                this.client_tel = client_tel;
                this.client_adresse = client_adresse;
                this.client_createur = client_createur;
                this.client_solde = 0;

                if(client_createur == true)
                {
                    co.Requete("INSERT INTO createur_de_recettes (nom_cdr,solde_cdr) VALUES ('" + client_nom + "'," + client_solde + ");");
                    co.Requete("INSERT INTO client (nom_client, tel_client, adresse_client, nom_cdr) VALUES ('" + client_nom + "', '" + client_tel + "', '" + client_adresse + "', '" + client_nom + "');");
                }
                else
                {
                    co.Requete("INSERT INTO client (nom_client, tel_client, adresse_client) VALUES ('" + client_nom + "', '" + client_tel + "', '" + client_adresse + "');");
                }
            }

            //Proprietes
            public string Client_nom
            {
                get { return this.client_nom; }
            }

            public string Client_tel
            {
                get { return this.client_tel; }
            }

            public string Client_adresse
            {
                get { return this.client_adresse; }
            }

            public bool Client_createur
            {
                get { return this.client_createur; }
            }
            public float Client_solde
            {
                get { return this.client_solde; }
            }

            //Methodes 
            public override string ToString()
            {
                if (client_createur) { return "Client : " + client_nom + " est un créateur." + "\nTél : " + client_tel; }
                else return "Client : " + client_nom + " n'est pas un créateur." + "\nTél : " + client_tel;
            }
            /// <summary>
            /// Fonction permettant de modifier le numéro de téléphone du Client et de le mettre à jour dans la base de donnée
            /// </summary>
            /// <param name="tel"></param>
            /// <param name="co"></param>
            public void Modifier_Tel(string tel, CoSQL co)
            {
                while(tel.Length != 10)
                {
                    Console.WriteLine("Erreur de format : entrez un numéro à 10 chiffres. Reessayez");
                    tel = Convert.ToString(Console.ReadLine());
                }
                co.Requete("UPDATE client SET tel_client = '" + tel + "' WHERE nom_client = '" + this.client_nom + "';");
                this.client_tel = tel;
            }
            /// <summary>
            /// Fonction permettant de modifier l'adresse du Client et de le mettre à jour dans la base de donnée
            /// </summary>
            /// <param name="adresse"></param>
            /// <param name="co"></param>
            public void Modifier_adresse(string adresse, CoSQL co)
            {
                co.Requete("UPDATE client SET adresse_client = '" + adresse + "' WHERE nom_client = '" + this.client_nom + "';");
                this.client_adresse = adresse;
            }
            /// <summary>
            /// Permet au client de devenir créateur de recette où de ne plus l'être
            /// </summary>
            /// <param name="cdr"></param>
            /// <param name="co"></param>
            public void Modifier_cdr(bool cdr, CoSQL co)
            {
                if(cdr == true && this.client_createur == false)
                {
                    co.Requete("INSERT INTO createur_de_recettes (nom_cdr,solde_cdr) VALUES ('" + this.client_nom + "'," + this.client_solde + ");");
                    co.Requete("UPDATE client SET nom_cdr = '" + this.client_nom + "' WHERE nom_client = '" + this.client_nom + "';");
                }
                if(cdr == false && this.client_createur == true)
                {
                    co.Requete("UPDATE client SET nom_cdr = NULL  WHERE nom_client = '" + this.client_nom + "';");
                    co.Requete("DELETE FROM commande WHERE nom_recette in (SELECT nom_recette FROM recette WHERE nom_cdr = '"+this.client_nom+"');");
                    co.Requete("DELETE FROM contient WHERE nom_recette in (SELECT nom_recette FROM recette WHERE nom_cdr = '"+this.client_nom+"');");
                    co.Requete("DELETE FROM recette WHERE nom_cdr = '" + this.client_nom + "';");
                    co.Requete("DELETE FROM createur_de_recettes WHERE nom_cdr = '" + this.client_nom + "';");
                }
                if(cdr == true && this.client_createur == true)
                {
                    Console.WriteLine("Le client est déjà créateur de recette");
                }
                if (cdr == false && this.client_createur == false)
                {
                    Console.WriteLine("Le client n'est déjà qu'un client uniquement");
                }
                this.client_createur = cdr;
            }
            /// <summary>
            /// Fonction permettant de mettre à jour le solde d'un créateur de recette dans la base de donnée
            /// </summary>
            /// <param name="solde"></param>
            /// <param name="co"></param>
            public void Modifier_solde(float solde, CoSQL co)
            {
                co.Requete("UPDATE createur_de_recettes SET solde_cdr = '" + Convert.ToString(solde) + "' WHERE nom_cdr = '" + this.client_nom + "';");
                this.client_solde = solde;
            }
            /// <summary>
            /// Fonction permettant de faire la rémnunération du cdr
            /// </summary>
            /// <param name="r"></param>
            /// <param name="co"></param>
            public void Remuneration(Recette r, CoSQL co)
            {
                this.client_solde += r.Cdr_remuneration;
                co.Requete("UPDATE createur_de_recettes SET solde_cdr = '" + this.Client_solde + "' WHERE nom_cdr = '" + this.client_nom + "';");
            }
            /// <summary>
            /// Fonction permettant de supprimer le client
            /// </summary>
            /// <param name="co"></param>
            public void Supprimer(CoSQL co)
            {
                co.Requete("DELETE FROM client WHERE nom_client = '" + this.client_nom + "';");
            }

        }
        /// <summary>
        /// Classe fournisseur
        /// </summary>
        public class Fournisseur
        {
            //Atributs
            private string fournisseur_nom;
            private string fournisseur_tel;

            //Constructeurs
            /// <summary>
            /// Constructeur permettant de récupérer les informations d'un fournisseur de la base de donnée
            /// </summary>
            /// <param name="fournisseur_nom"></param>
            /// <param name="co"></param>
            public Fournisseur(string fournisseur_nom, CoSQL co)  //créé un objet fournisseur deja dans la BDD SQL
            {
                this.fournisseur_nom = fournisseur_nom;
                this.fournisseur_tel = co.Requete_lecture("SELECT tel_f FROM fournisseur WHERE nom_f = '" + fournisseur_nom + "'");
            }
            /// <summary>
            /// Constructeur permettant de créer un nouveau fournisseur et de l'insérer dans la BDD
            /// </summary>
            /// <param name="fournisseur_nom"></param>
            /// <param name="fournisseur_tel"></param>
            /// <param name="co"></param>
            public Fournisseur(string fournisseur_nom, string fournisseur_tel, CoSQL co) //Créé un nouveau fournisseur
            {
                this.fournisseur_nom = fournisseur_nom;
                this.fournisseur_tel = fournisseur_tel;

                co.Requete("INSERT INTO fournisseur (nom_f, tel_f) VALUES ('" + fournisseur_nom + "', '" + fournisseur_tel + "');");
            }

            //Proprietes
            public string Fournisseur_nom
            {
                get { return this.fournisseur_nom; }
            }
            public string Fournisseur_tel
            {
                get { return this.fournisseur_tel; }
            }
            /// <summary>
            /// Fonction permettant d'avoir les informations du fournisseur
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "Fournisseur : " + fournisseur_nom + "\nTél : " + fournisseur_tel;
            }
            /// <summary>
            /// Fonction permettant de supprimer un fournisseur de la base de donnée
            /// </summary>
            /// <param name="co"></param>
            public void Supprimer(CoSQL co)
            {
                co.Requete("DELETE FROM fournisseur WHERE nom_f = '" + this.fournisseur_nom + "';");
            }
        }
        /// <summary>
        /// Classe Produit
        /// </summary>
        public class Produit
        {
            //Attributs
            private string produit_nom;
            private string produit_categorie;
            private string unite_quantite;
            private int stock_actuel;
            private int stock_min;
            private int stock_max;
            private string fournisseur_nom;
            private string fournisseur_reference;

            //Constructeurs
            /// <summary>
            /// Constructeur permettant de récupérer les informations d'un produit de la BDD
            /// </summary>
            /// <param name="produit_nom"></param>
            /// <param name="co"></param>
            public Produit(string produit_nom, CoSQL co)
            {
                this.produit_nom = produit_nom;
                this.produit_categorie = co.Requete_lecture("SELECT categorie_p FROM produit WHERE nom_produit='"+produit_nom+"';");
                this.unite_quantite = co.Requete_lecture("SELECT unite_quantite FROM produit WHERE nom_produit='" + produit_nom + "';");
                this.stock_actuel = int.Parse(co.Requete_lecture("SELECT stock_actuel FROM produit WHERE nom_produit='" + produit_nom + "';"));
                this.stock_min = int.Parse(co.Requete_lecture("SELECT stock_min FROM produit WHERE nom_produit='" + produit_nom + "';"));
                this.stock_max = int.Parse(co.Requete_lecture("SELECT stock_max FROM produit WHERE nom_produit='" + produit_nom + "';"));
                this.fournisseur_nom = co.Requete_lecture("SELECT nom_fournisseur FROM produit WHERE nom_produit='" + produit_nom + "';");
                this.fournisseur_reference = co.Requete_lecture("SELECT ref_fournisseur FROM produit WHERE nom_produit='" + produit_nom + "';");
            }
            /// <summary>
            /// Constructeur permettant de créer un produit de puis C# et de l'ajouter à la BDD
            /// </summary>
            /// <param name="produit_nom"></param>
            /// <param name="produit_categorie"></param>
            /// <param name="unite_quantite"></param>
            /// <param name="stock_actuel"></param>
            /// <param name="stock_min"></param>
            /// <param name="stock_max"></param>
            /// <param name="fournisseur_nom"></param>
            /// <param name="fournisseur_reference"></param>
            /// <param name="co"></param>
            public Produit(string produit_nom, string produit_categorie, string unite_quantite, int stock_actuel, int stock_min, int stock_max, string fournisseur_nom, string fournisseur_reference,CoSQL co)
            {
                this.produit_nom = produit_nom;
                this.produit_categorie = produit_categorie;
                this.unite_quantite = unite_quantite;
                this.stock_actuel = stock_actuel;
                this.stock_min = stock_min;
                this.stock_max = stock_max;
                this.fournisseur_nom = fournisseur_nom;
                this.fournisseur_reference = fournisseur_reference;

                co.Requete("INSERT INTO produit VALUES ('" + produit_nom + "', '" + produit_categorie + "', '" + unite_quantite + "', '" + stock_actuel + "', '" + stock_min + "', '" + stock_max + "', '" + fournisseur_nom + "', '" + fournisseur_reference + "');");
            }

            //Proprietes
            public string Produit_nom
            {
                get { return this.produit_nom; }
            }
            public string Produit_categorie
            {
                get { return this.produit_categorie; }
            }
            public string Unite_quantite
            {
                get { return this.unite_quantite; }
            }
            public int Stock_actuel
            {
                get { return this.stock_actuel; }
            }
            public int Stock_min
            {
                get { return this.stock_min; }
            }
            public int Stock_max
            {
                get { return this.stock_max; }
            }
            public string Fournisseur_nom
            {
                get { return this.fournisseur_nom; }
            }
            public string Fournisseur_reference
            {
                get { return this.fournisseur_reference; }
            }

            //Methodes
            /// <summary>
            /// Requete permettant de modifier le stock actuel d'un produit dans la BDD
            /// </summary>
            /// <param name="stock"></param>
            /// <param name="co"></param>
            public void Modifier_stock_actuel(int stock, CoSQL co)
            {
                co.Requete("UPDATE produit SET stock_actuel = '" + stock + "' WHERE nom_produit = '" + this.produit_nom + "';");
                this.stock_actuel = stock;
            }
            /// <summary>
            /// Modifier le stock minimal d'un produit dans la BDD
            /// </summary>
            /// <param name="stock"></param>
            /// <param name="co"></param>
            public void Modifier_stock_min(int stock, CoSQL co)
            {
                co.Requete("UPDATE produit SET stock_min = '" + stock + "' WHERE nom_produit = '" + this.produit_nom + "';");
                this.stock_min = stock;
            }
            /// <summary>
            /// Modifier le stock maximal d'un produit dans la BDD
            /// </summary>
            /// <param name="stock"></param>
            /// <param name="co"></param>
            public void Modifier_stock_max(int stock, CoSQL co)
            {
                co.Requete("UPDATE produit SET stock_max = '" + stock + "' WHERE nom_produit = '" + this.produit_nom + "';");
                this.stock_max = stock;
            }
            /// <summary>
            /// Permet d'afficher toutes les informations d'un produit
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "Produit : " + produit_nom + "\nCatégorie : " + produit_categorie + "\nUnité de quantité : " + unite_quantite + "\nStock actuel : " + stock_actuel + "\nStock minimum :" + stock_min + "\nStock Maximum :" + stock_max + "\nFournisseur : " + fournisseur_nom + "\nRéférence fournisseur : " + fournisseur_reference;
            }
            /// <summary>
            /// permet de supprimer un produit
            /// </summary>
            /// <param name="co"></param>
            public void Supprimer(CoSQL co)
            {
                co.Requete("DELETE FROM produit WHERE nom_produit = '" + this.produit_nom + "';");
            }
            
        }
        public class Recette 
        {
            //Attributs
            private string recette_nom = "";
            private string recette_type = "";
            private string recette_descriptif = "";
            private float recette_prix = 0;
            private string nom_cdr = "";
            private List<Produit> produits;
            private List<int> quantites;
            private int nb_commande=0;
            private int cdr_remuneration = 0;

            //Constructeur
            /// <summary>
            /// Constructeur permettant de récupérer les information d'une recette depuis la base de donnée
            /// </summary>
            /// <param name="recette_nom"></param>
            /// <param name="co"></param>
            public Recette(string recette_nom,CoSQL co)
            {
                this.recette_nom = recette_nom;
                this.recette_type = co.Requete_lecture("SELECT type_recette FROM recette WHERE nom_recette='"+this.recette_nom+"';");
                this.recette_descriptif = co.Requete_lecture("SELECT descriptif_recette FROM recette WHERE nom_recette='" + this.recette_nom + "';");
                this.recette_prix = float.Parse(co.Requete_lecture("SELECT prix_vente FROM recette WHERE nom_recette='" + this.recette_nom + "';"));
                this.nom_cdr = co.Requete_lecture("SELECT nom_cdr FROM recette WHERE nom_recette='" + this.recette_nom + "';");
                int nb_produits = int.Parse(co.Requete_lecture("SELECT count(*) FROM contient WHERE nom_recette = '"+this.recette_nom+"';"));
                List<Produit> pro = new List<Produit>();
                for (int i = 0; i < nb_produits; i++)
                {
                    Produit p = new Produit(co.Requete_lecture("SELECT nom_produit FROM contient WHERE nom_recette ='" + this.recette_nom + "' LIMIT 1 OFFSET " + i + ";"), co);
                    pro.Add(p);
                }
                this.produits = pro;
                List<int> qte = new List<int>();
                for (int i = 0; i < nb_produits; i++)
                {
                    qte.Add(int.Parse(co.Requete_lecture("SELECT quantite_p FROM contient WHERE nom_recette ='"+this.recette_nom+"' LIMIT 1 OFFSET "+i+";")));
                }
                this.quantites = qte;
                this.nb_commande = int.Parse(co.Requete_lecture("SELECT nb_commande FROM recette WHERE nom_recette ='"+this.recette_nom+"';"));
                if(this.nb_commande <=50)
                {
                    this.cdr_remuneration = 2;
                }
                else if(this.nb_commande <50)
                {
                    this.cdr_remuneration = 4;
                }

            }
            /// <summary>
            /// Constructeur permettant de créer une recette depuis C# et de l'intégrer à la BDD
            /// </summary>
            /// <param name="recette_nom"></param>
            /// <param name="recette_type"></param>
            /// <param name="recette_descriptif"></param>
            /// <param name="recette_prix"></param>
            /// <param name="nom_cdr"></param>
            /// <param name="produits"></param>
            /// <param name="quantites"></param>
            /// <param name="co"></param>
            public Recette(string recette_nom, string recette_type, string recette_descriptif, float recette_prix, string nom_cdr, List<Produit> produits, List<int> quantites, CoSQL co)
            {
                this.recette_nom = recette_nom;
                this.recette_type = recette_type;
                this.recette_descriptif = recette_descriptif;
                this.recette_prix = recette_prix;
                this.nom_cdr = nom_cdr;
                this.produits = produits;
                this.quantites = quantites;

                co.Requete("INSERT INTO recette VALUES ('" + recette_nom + "', '" + recette_type + "', '" + recette_descriptif + "', '" + recette_prix + "', '" + nom_cdr + "',0);");
                for(int i=0; i<produits.Count; i++)
                {
                    co.Requete("INSERT INTO contient VALUES('"+recette_nom+"','"+produits[i].Produit_nom+"', "+quantites[i]+")");
                }
                this.cdr_remuneration = 2;
            }

            public string Recette_nom
            {
                get { return recette_nom; }
            }
            public string Recette_type
            {
                get { return recette_type; }
            }
            public string Recette_descriptif
            {
                get { return recette_descriptif; }
            }
            public float Recette_prix
            {
                get { return recette_prix; }
            }
            public string Nom_cdr
            {
                get { return nom_cdr; }
            }
            public int Nb_commande
            {
                get { return this.nb_commande; }
            }
            public int Cdr_remuneration
            {
                get { return this.cdr_remuneration; }
                set { this.cdr_remuneration = value; }
            }
            public List<Produit> Recette_ingredients
            {
                get { return this.produits; }
            }
            public List<int> Recette_quantite
            {
                get { return this.quantites; }
            }
            /// <summary>
            /// Permet d'afficher les informations d'une recette
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "Recette : " + recette_nom + "\nType : " + recette_type + "\nDescriptif : " + recette_descriptif + "\nPrix :" + recette_prix + "\nNom du créateur : " + nom_cdr;
            }
            /// <summary>
            /// Permet de modifier le prix d'une recette
            /// </summary>
            /// <param name="prix"></param>
            /// <param name="co"></param>
            public void Modifier_prix(float prix,CoSQL co)
            {
                co.Requete("UPDATE recette SET prix_vente = '" + prix + "' WHERE nom_recette = '" + this.recette_nom+ "';");
                this.recette_prix = prix;
            }
            /// <summary>
            /// Permet d'ajouter une commande 
            /// </summary>
            /// <param name="co"></param>
            public void Ajouter_commande(CoSQL co)
            {
                this.nb_commande++;
                co.Requete("UPDATE recette SET nb_commande = '" + this.nb_commande + "' WHERE nom_recette = '" + this.recette_nom + "';");
            }
            /// <summary>
            ///  Permet de supprimer une recette de la BDD ainsi que toutes les commandes qui lui sont liées
            /// </summary>
            /// <param name="co"></param>
            public void Supprimer(CoSQL co)
            {
                co.Requete("DELETE FROM contient WHERE nom_recette = '" + this.recette_nom + "';");
                co.Requete("DELETE FROM commande WHERE nom_recette = '" + this.recette_nom + "';");
                co.Requete("DELETE FROM recette WHERE nom_recette = '" + this.recette_nom + "';");
            }
        }

        public class CoSQL
        {

            private MySqlConnection connection;

            // Constructeur
            public CoSQL(string mdp)
            {
                this.InitConnexion(mdp);
            }
            // Méthode pour initialiser la connexion
            /// <summary>
            /// Creation de la connexion en utilisant le mot de passe
            /// </summary>
            /// <param name="mdp"></param>
            // Méthode pour initialiser la connexion
            private void InitConnexion(string mdp)
            {
                // Création de la chaîne de connexion
                string connectionString = "SERVER=127.0.0.1; DATABASE=cooking; UID=root; PASSWORD="+mdp;
                this.connection = new MySqlConnection(connectionString);
            }
            /// <summary>
            /// Fonction permettant d'exécuter une requete simple avec une récupération si une erreur s'est produite
            /// </summary>
            /// <param name="requete"></param>
            public void Requete(string requete)
            {
                try
                {
                    // Ouverture de la connexion SQL
                    this.connection.Open();

                    // Création d'une commande SQL en fonction de l'objet connection
                    MySqlCommand cmd = this.connection.CreateCommand();

                    // Requête SQL
                    cmd.CommandText = requete;

                    // Exécution de la commande SQL
                    cmd.ExecuteNonQuery();

                    // Fermeture de la connexion
                    this.connection.Close();
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(" ErreurConnexion : " + e.ToString());
                    Console.ReadKey();
                    return;
                }
            }
            /// <summary>
            /// Permet d'effectuer une requette de lecture de la base de donnée afin de stocker dans une chaine de caractère les informations récupérés
            /// </summary>
            /// <param name="requete">les informations retournés par la requete SQL</param>
            /// <returns></returns>
            public string Requete_lecture(string requete)
            {
                this.connection.Open();
                MySqlDataReader reader;
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = requete; // requete SQL
                //command.Parameters.AddWithValue("@nom_cdr", connection.Requete_lecture(nom_cdr));
                reader = command.ExecuteReader();
                string sortie = "";
                while (reader.Read())                           // parcours ligne par ligne
                {
                    string currentRowAsString = "";
                    for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                    {
                        string valueAsString = reader.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                        currentRowAsString += valueAsString + " ";
                    }
                    sortie += currentRowAsString + "\n";
                    //Console.WriteLine(currentRowAsString);    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
                }
                connection.Close();
                return Strip(sortie);
            }        
        }
        /// <summary>
        /// Permet d'enlever le caractère espace et retour à la ligne présente après chaque lecture de la BDD
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        static string Strip(string txt)
        {
            string test = "";
            for (int i = 0; i < txt.Length - 2; i++)
            {
                test += txt[i];
            }
            return test;
        }
        /// <summary>
        /// Permet au client de saisir son nom afin de se connecter à la base de donnée
        /// Si le client n'existe pas le nouveau client sera invité à s'inscrire
        /// </summary>
        /// <param name="co"></param>
        /// <returns></returns>
        static Client Connexion(CoSQL co)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("=== Connexion ===");
            Console.ResetColor();
            Console.Write("Nom :");
            string nom = Convert.ToString(Console.ReadLine());
            if(co.Requete_lecture("SELECT count(*) FROM client WHERE nom_client = '"+nom+"';") == "1")
            {
                Client c1 = new Client(nom,co);
                return c1;
            }
            else
            {
                Client c1 = Inscription(co);
                return c1;
            }
        }
        /// <summary>
        /// Fonction permettant au client de s'inscire et permet à la base de donnée de récupérer ses informations
        /// </summary>
        /// <param name="co"></param>
        /// <returns></returns>
        static Client Inscription(CoSQL co)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("=== Inscription ===");
            Console.ResetColor();
            Console.Write("Nom : ");
            string client_nom = Convert.ToString(Console.ReadLine());
            int nb_nom = int.Parse(co.Requete_lecture("SELECT count(*) FROM client WHERE nom_client LIKE '"+client_nom+ "%';"));
            if (nb_nom != 0)
            {
                client_nom = client_nom + "_" + nb_nom;
                Console.WriteLine("Votre nom sera : "+client_nom+" car il existe déjà des personnes avec ce nom dans la base de donée");
            }
            Console.Write("Numéro de téléphone : ");
            string client_tel = Convert.ToString(Console.ReadLine());
            Console.Write("Adresse (n° voie CP Ville) : ");
            string client_adresse = Convert.ToString(Console.ReadLine());
            Console.Write("Etes-vous un créateur de recette (vous devrez alors créer une recette) ? (Oui/Non) " );
            string createur = Convert.ToString(Console.ReadLine());
            createur = createur.ToUpper();
            bool crea;
            if (createur == "OUI")
            {
                crea = true;
            }
            else
            {
                crea = false;
            }
            Client c1 = new Client(client_nom, client_tel, client_adresse, crea, co);
            return c1;
        }
        /// <summary>
        /// Fonction permettant de payer sa commande avec une carte Bankcook ou de déduire le solde du CDR
        /// </summary>
        /// <param name="prix"></param>
        /// <param name="client"></param>
        /// <param name="co"></param>
        /// <returns></returns>
        static bool Payement(float prix, Client client, CoSQL co)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("=== PAYEMENT ===");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Vous devez payer le prix de : " + prix + " cook");
            if (client.Client_createur)
            {
                Console.WriteLine("Vous avez un solde de : " + client.Client_solde + "cook");
                Console.WriteLine("Voulez-vous utiliser votre solde pour payer (ou obtenir une réduction) ? (Oui/Non)");
                string rep = Convert.ToString(Console.ReadLine());
                rep = rep.ToUpper();
                if (rep == "OUI")
                {
                    float nouv_prix = prix - client.Client_solde;
                    if (nouv_prix < 0)
                    {
                        prix = 0;
                        client.Modifier_solde(-nouv_prix,co);
                        Console.WriteLine("Merci ! Le payement a bien été effectué. Il vous reste : " + client.Client_solde + "cook sur votre solde");
                        Console.WriteLine("Appuyez sur entrer pour continuer");
                        Console.ReadKey();
                        return true;
                    }
                    else
                    {
                        prix = nouv_prix;
                        client.Modifier_solde(0, co);
                        Console.WriteLine("Le nouveau prix est de " + prix + "cook. Vous n'avez plus de cook sur votre solde");
                        Console.WriteLine("Payer ? (Oui/Non)");
                        string payer = Convert.ToString(Console.ReadLine());
                        payer = payer.ToUpper();
                        if (payer == "OUI")
                        {
                            Console.WriteLine("Indiquez votre numéro de carte bancaire BANKCOOK :");
                            string carte = Convert.ToString(Console.ReadLine());
                            Console.WriteLine("Merci ! Le payement a bien été effectué.");
                            Console.WriteLine("Appuyez sur entrer pour continuer");
                            Console.ReadKey();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Payer ? (Oui/Non)");
                    string payer = Convert.ToString(Console.ReadLine());
                    payer = payer.ToUpper();
                    if (payer == "OUI")
                    {
                        Console.WriteLine("Indiquez votre numéro de carte bancaire BANKCOOK :");
                        string carte = Convert.ToString(Console.ReadLine());
                        Console.WriteLine("Merci ! Le payement a bien été effectué.");
                        Console.WriteLine("Appuyez sur entrer pour continuer");
                        Console.ReadKey();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                Console.WriteLine("Payer ? (Oui/Non)");
                string payer = Convert.ToString(Console.ReadLine());
                payer = payer.ToUpper();
                if (payer == "OUI")
                {
                    Console.WriteLine("Indiquez votre numéro de carte bancaire BANKCOOK :");
                    string carte = Convert.ToString(Console.ReadLine());
                    Console.WriteLine("Merci ! Le payement a bien été effectué.");
                    Console.WriteLine("Appuyez sur entrer pour continuer");
                    Console.ReadKey();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Permet à un client de passer une ciommande
        /// </summary>
        /// <param name="client"></param>
        /// <param name="co"></param>
        static void Commande(Client client, CoSQL co)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("### COMMANDE ###");
            Console.ResetColor();
            Console.WriteLine();
            int num_commande = int.Parse(co.Requete_lecture("SELECT distinct MAX(num_commande) FROM commande;")) + 1;
            float prix = 0;
            bool fini = false;
            List<Recette> recettes_commandees = new List<Recette> { };
            List<int> qte_commandees = new List<int> { };
            DateTime time = DateTime.Now;
            while (fini == false)
            {
                Console.WriteLine("Voici la liste des recettes :");
                int nb_recette = int.Parse(co.Requete_lecture("SELECT count(distinct r.nom_recette) FROM recette r, contient c, produit p WHERE r.nom_recette = c.nom_recette AND c.nom_produit = p.nom_produit AND p.stock_actuel>=p.stock_min;"));
                for (int i = 0; i < nb_recette; i++)
                {
                    Console.Write(i + " - ");
                    string recette = co.Requete_lecture("SELECT distinct r.nom_recette FROM recette r, contient c, produit p WHERE r.nom_recette = c.nom_recette AND c.nom_produit = p.nom_produit AND p.stock_actuel>=p.stock_min LIMIT 1 OFFSET " + i + ";");
                    Console.Write(recette);
                    Console.WriteLine(" - prix : " + co.Requete_lecture("SELECT prix_vente FROM recette WHERE nom_recette ='"+recette+"';") + "cook");
                }
                Console.WriteLine();
                Console.WriteLine("Quelle recette souhaitez-vous choisir ?");
                int choix = Convert.ToInt32(Console.ReadLine());
                string r_choisie = co.Requete_lecture("SELECT distinct r.nom_recette FROM recette r, contient c, produit p WHERE r.nom_recette = c.nom_recette AND c.nom_produit = p.nom_produit AND p.stock_actuel>=p.stock_min LIMIT 1 OFFSET " + choix + ";");
                bool dejadedans = false ;
                for (int i = 0; i < recettes_commandees.Count; i++)
                {
                    if (r_choisie == recettes_commandees[i].Recette_nom)
                    {
                        dejadedans = true;
                    }
                }
                if(dejadedans == true)
                {
                    do
                    {
                        Console.WriteLine("Recette déjà dans la commande. Quelle recette souhaitez-vous choisir ?");
                        choix = Convert.ToInt32(Console.ReadLine());
                        r_choisie = co.Requete_lecture("SELECT distinct r.nom_recette FROM recette r, contient c, produit p WHERE r.nom_recette = c.nom_recette AND c.nom_produit = p.nom_produit AND p.stock_actuel>p.stock_min LIMIT 1 OFFSET " + choix + ";");
                        dejadedans = false;
                        for (int i = 0; i < recettes_commandees.Count; i++)
                        {
                            if (r_choisie == recettes_commandees[i].Recette_nom)
                            {
                                dejadedans = true;
                            }
                        }
                    } while (dejadedans == true);
                }
                Recette recette_choisie = new Recette(r_choisie, co);
                recettes_commandees.Add(recette_choisie);
                Console.WriteLine("Combien de "+r_choisie +" voulez-vous commander ?");
                int qte = Convert.ToInt32(Console.ReadLine());
                qte_commandees.Add(qte);
                prix += qte*recette_choisie.Recette_prix;
                Console.WriteLine("Voulez-vous commander un autre plat ? (Oui/Non)");
                string autre = Console.ReadLine();
                autre = autre.ToUpper();
                if(autre == "NON")
                {
                    fini = true;
                }
                if(recettes_commandees.Count == nb_recette)
                {
                    Console.WriteLine("Vous avez commandé toutes les recettes de la liste");
                    fini = true;
                }
            }
            bool valide = false;
            while (valide == false)
            {
                valide = Payement(prix, client,co);
            }

            int nb_recette_commandee = recettes_commandees.Count;
            for(int i=0;i<nb_recette_commandee;i++)
            {
                string date = time.Year + "-" + time.Month + "-" + time.Day;
                co.Requete("INSERT INTO commande VALUES('" + recettes_commandees[i].Recette_nom + "'," + qte_commandees[i] + ", '" + client.Client_nom + "', " + num_commande + ", '" + date + "');");
                for(int j=0;j<qte_commandees[i];j++)
                {
                    recettes_commandees[i].Ajouter_commande(co);
                }
                string nom_cdr = co.Requete_lecture("SELECT nom_cdr FROM recette WHERE nom_recette = '" + recettes_commandees[i].Recette_nom + "';");
                Client cdr = new Client(nom_cdr, co);
                for (int j = 0; j < qte_commandees[i]; j++)
                {
                    cdr.Remuneration(recettes_commandees[i], co);
                }
                if (recettes_commandees[i].Nb_commande == 51)
                {
                    recettes_commandees[i].Modifier_prix(recettes_commandees[i].Recette_prix + 5, co);
                    recettes_commandees[i].Cdr_remuneration = 4;
                }
                else if (recettes_commandees[i].Nb_commande == 11)
                {
                    recettes_commandees[i].Modifier_prix(recettes_commandees[i].Recette_prix + 2, co);
                }
                List<Produit> produits_consom = recettes_commandees[i].Recette_ingredients;
                List<int> quantite_consom = recettes_commandees[i].Recette_quantite;
                for(int j=0;j<produits_consom.Count;j++)
                {
                    produits_consom[j].Modifier_stock_actuel(produits_consom[j].Stock_actuel - quantite_consom[j]*qte_commandees[i], co);
                }
            }
        }
        /// <summary>
        /// Permet de vérifier l'existance d'un produit dans la base de donnée 
        /// </summary>
        /// <param name="produit"></param>
        /// <param name="co"></param>
        /// <returns></returns>
        static bool Verifier_Produit(string produit, CoSQL co)
        {
            bool present = false;
            string recherche = co.Requete_lecture("SELECT count(*) FROM cooking.produit WHERE nom_produit = '" + produit + "';");
            if (recherche == "1")
            {
                present = true;
            }
            return present;
        }
        /// <summary>
        /// NON DEMANDE: Permet de vérifier l'existance d'un fournisseur dans la base de donnée 
        /// </summary>
        /// <param name="nom_fournisseur"></param>
        /// <param name="co"></param>
        /// <returns></returns>
        static bool Verifier_Fournisseur(string nom_fournisseur, CoSQL co)
        {
            bool present = false;
            string recherche = co.Requete_lecture("SELECT count(*) FROM cooking.fournisseur WHERE nom_fournisseur = '" + nom_fournisseur + "';");
            if (recherche == "1")
            {
                present = true;
            }
            return present;
        }
        /// <summary>
        /// NON DEMANDE: Permet de créer un fournisseur
        /// </summary>
        /// <param name="fournisseur"></param>
        /// <param name="co"></param>
        static void Creation_Fournisseur(string fournisseur, CoSQL co)
        {
            Console.WriteLine("Quel est le numéro de téléphone du fournisseur:");
            string num = Console.ReadLine();
            Fournisseur f = new Fournisseur(fournisseur, num, co);
        }
        /// <summary>
        /// Fonction permettant de créer un nouveau produit
        /// De choisir le fournisseur de son produit parmis une liste de fournisseur
        /// </summary>
        /// <param name="produit_nom"></param>
        /// <param name="co"></param>
        /// <returns></returns>
        static Produit Creation_produit(string produit_nom, CoSQL co)
        {
            Console.WriteLine("Quel est la catégorie du produit : ");
            string produit_categorie = Console.ReadLine();
            Console.WriteLine("Quelle est l'unité de quantitée (ex : kg,pce,L...) : ");
            string unite_quantite = Console.ReadLine();
            Console.WriteLine("Voici la liste des fournisseurs :");
            int nb_four = int.Parse(co.Requete_lecture("SELECT count(*) FROM fournisseur;"));
            for (int i = 0; i < nb_four; i++)
            {
                Console.Write(i + " - ");
                Console.WriteLine(co.Requete_lecture("SELECT nom_f FROM fournisseur LIMIT 1 OFFSET " + i + ";"));
            }
            Console.WriteLine("Quel fournisseur voulez-vous choisir ? ");
            int choix = Convert.ToInt32(Console.ReadLine());
            string f_choisi = co.Requete_lecture("SELECT nom_f FROM fournisseur LIMIT 1 OFFSET " + choix + ";");
            Console.WriteLine("Quel est la reference du produit (ex: Sauce Bolognaise) ref = BOL : ");
            string fournisseur_reference = Console.ReadLine();
            while(co.Requete_lecture("SELECT count(*) FROM produit WHERE ref_fournisseur ='"+fournisseur_reference+"'") != "0")
            {
                Console.WriteLine("Cette reférence fournisseur est déjà utilisée. Entrez-en une nouvelle :");
                fournisseur_reference = Console.ReadLine();
            }
            Produit p1 = new Produit(produit_nom, produit_categorie.ToUpper(), unite_quantite, 0, 10, 200, f_choisi, fournisseur_reference, co);
            return p1;
        }
        /// <summary>
        /// Fonction permettant au CDR de créer une novuelle recette
        /// </summary>
        /// <param name="client"></param>
        /// <param name="co"></param>
        static void Creation_recette(Client client, CoSQL co) //Creer recette
        {
            Console.WriteLine("--- Création d'une nouvelle recette ---");
            Console.WriteLine("Quelle est le nom de la recette: ");
            string recette_nom = Console.ReadLine();
            Console.WriteLine("Quelle est le type de la recette: ");
            string recette_type = Console.ReadLine();
            Console.WriteLine("Quelle est le descriptif de la recette: ");
            string recette_descriptif = Console.ReadLine();
            Console.WriteLine("Quelle est le prix de la recette: ");
            float recette_prix = float.Parse(Console.ReadLine());
            string nom_cdr = client.Client_nom;
            int i = 1;
            List<Produit> produits = new List<Produit> {};
            List<int> quantites = new List<int> {};
            string choix = "";
            string produit = "";
            do
            {
                Console.WriteLine("Quel est le nom du produit " + i + " : ");
                produit = Console.ReadLine();
                if (Verifier_Produit(produit, co))
                {
                    Produit p1 = new Produit(produit, co);
                    produits.Add(p1);
                }
                else
                {
                    Produit p1 = Creation_produit(produit, co);
                    produits.Add(p1);
                }
                Console.WriteLine("Quel est la quantité nécessaire dans la recette pour ce produit :");
                quantites.Add(Convert.ToInt32(Console.ReadLine()));
                Console.WriteLine("Voulez vous ajouter un autre produit ? ");
                choix = Console.ReadLine();
                choix = choix.ToUpper();
                i++;
            } while (choix != "NON");
            Recette r1 = new Recette(recette_nom, recette_type, recette_descriptif, recette_prix, nom_cdr, produits, quantites,co);
        }
        /// <summary>
        /// Espace du Créateur de recette permettant de faire la gestion de ses recettes
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="co"></param>
        static void Espace_CdR(Client c1,CoSQL co)
        {
            bool escp = false;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("=== Espace Créateur de recettes ===");
                Console.ResetColor();
                Console.WriteLine("Voulez voulez :");
                Console.WriteLine("1 - Créer une nouvelle recette");
                Console.WriteLine("2 - Consulter votre solde");
                Console.WriteLine("3 - Afficher le nombre de vente de vos recettes");
                Console.WriteLine("4 - Sortir de l'espace");
                Console.Write("Choix : ");
                int action_cdr = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();
                switch (action_cdr)
                {
                    case 1:
                        Creation_recette(c1, co);
                        break;
                    case 2:
                        Console.Write("Votre solde est de : ");
                        Console.WriteLine(c1.Client_solde + "cook");
                        break;
                    case 3:
                        int nb = int.Parse(co.Requete_lecture("SELECT count(*) FROM recette WHERE nom_cdr = '" + c1.Client_nom + "';"));
                        Console.WriteLine("Vos recettes : ");
                        for(int i=0;i<nb;i++)
                        {
                            Console.Write(co.Requete_lecture("SELECT nom_recette FROM recette WHERE nom_cdr = '" + c1.Client_nom + "' GROUP BY(nb_commande) LIMIT 1 OFFSET "+i+";"));
                            Console.WriteLine(" - nombre de commande : " + co.Requete_lecture("SELECT nb_commande FROM recette WHERE nom_cdr = '" + c1.Client_nom + "' GROUP BY(nb_commande) LIMIT 1 OFFSET " + i + ";"));
                        }
                        break;
                    case 4:
                        escp = true;
                        break;
                }
                Console.WriteLine();
                Console.WriteLine("Appuyez sur entrer pour continuer");
                Console.ReadKey();
            } while (!escp);
        }
        /// <summary>
        /// Menu du Créateur de recette ainsi que les actions qu'il peut réaliser
        /// </summary>
        /// <param name="c"></param>
        /// <param name="co"></param>
        /// <returns></returns>
        static Client Menu_CdR(Client c,CoSQL co)
        {
            bool deco_CdR = false;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("=== Menu Créateur de recette ===");
                Console.ResetColor();
                Console.WriteLine("Voulez voulez :");
                Console.WriteLine("1 - acceder à votre espace de créateur de recette");
                Console.WriteLine("2 - Passer une commande");
                Console.WriteLine("3 - Deconnexion");
                Console.Write("Choix : ");
                int CdR = Convert.ToInt32(Console.ReadLine());
                switch (CdR)
                {
                    case 1:
                        Espace_CdR(c, co);
                        break;
                    case 2:
                        Commande(c, co);
                        break;
                    case 3:
                        deco_CdR = true;
                        break;
                }
            } while (!deco_CdR);
            return c;
        }
        /// <summary>
        /// Menu d'un client ainsi que les actions qu'il peut réaliser
        /// </summary>
        /// <param name="c"></param>
        /// <param name="co"></param>
        /// <returns></returns>
        static Client Menu_client(Client c,CoSQL co)
        {
            bool deco_C = false;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("=== Menu Client ===");
                Console.ResetColor();
                Console.WriteLine("Vous voulez: \n" +
                    "1 - Passer une commande \n" +
                    "2 - Devenir créateur de recette (créer une recette)\n" +
                    "3 - Deconnexion");
                Console.Write("Choix : ");
                int reponse = Convert.ToInt32(Console.ReadLine());
                switch (reponse)
                {
                    case 1:
                        Console.WriteLine("1");
                        Commande(c, co);
                        break;
                    case 2:
                        c.Modifier_cdr(true, co);
                        Creation_recette(c, co);
                        Menu_CdR(c,co);
                        deco_C = true;
                        break;
                    case 3:
                        deco_C = true;
                        break;
                }
            } while (deco_C == false);
            return c;
        }
        /// <summary>
        /// Menu pour le gérant Cooking
        /// </summary>
        /// <param name="co"></param>
        static void Menu_Cooking(CoSQL co)
        {
            bool deco_Cook = false;
            do
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("=== Menu COOKING ===");
                Console.ResetColor();
                Console.WriteLine("Que voulez vous faire ? \n" +
                    "1 - Afficher le tableau de bord de la semaine \n" +
                    "2 - Réapprovisionnement des produits \n" +
                    "3 - Supprimer une recette \n" +
                    "4 - Supprimer un CdR (et toute ses recettes) \n" +
                    "5 - Deconnexion");
                Console.Write("Choix : ");
                int gestion_cooking = Convert.ToInt32(Console.ReadLine());
                switch (gestion_cooking)
                {
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("--- Tableau de bord de la semaine ---");
                        Console.ResetColor();
                        Console.WriteLine();
                        Console.WriteLine("- CdR ayant été le plus commandé cette semaine :");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        string CommandText = "SELECT r.nom_cdr FROM recette r JOIN commande c ON r.nom_recette = c.nom_recette WHERE date_c > DATE_SUB(NOW(), INTERVAL 7 DAY) GROUP BY r.nom_cdr HAVING count(*) > ALL(SELECT count(*) FROM commande WHERE date_c > DATE_SUB(NOW(), INTERVAL 7 DAY) GROUP BY nom_recette) LIMIT 1;";
                        string CommandText2 = "SELECT count(r.nom_cdr) FROM recette r JOIN commande c ON r.nom_recette = c.nom_recette WHERE date_c > DATE_SUB(NOW(), INTERVAL 7 DAY) GROUP BY r.nom_cdr HAVING count(*) > ALL(SELECT count(*) FROM commande WHERE date_c > DATE_SUB(NOW(), INTERVAL 7 DAY) GROUP BY nom_recette) LIMIT 1;";
                        Console.WriteLine(co.Requete_lecture(CommandText) + " - " + co.Requete_lecture(CommandText2) + " commandes.");
                        Console.WriteLine();
                        Console.ResetColor();
                        Console.WriteLine("- Top 5 des recettes de la semaine (sous 7 jours) : ");
                        int top = int.Parse(co.Requete_lecture("SELECT count(distinct r.nom_recette) FROM recette r JOIN commande c ON r.nom_recette = c.nom_recette WHERE date_c > DATE_SUB(NOW(), INTERVAL 7 DAY);"));
                        if(top>5)
                        {
                            top = 5;
                        }
                        for (int i = 0; i < top; i++)
                        {
                            string nom = "SELECT r.nom_recette FROM recette r JOIN commande c ON r.nom_recette = c.nom_recette WHERE date_c > DATE_SUB(NOW(), INTERVAL 7 DAY) GROUP BY c.nom_recette ORDER BY count(c.nom_recette) DESC LIMIT 1 OFFSET " + i + "; ";
                            string type = "SELECT r.type_recette FROM recette r JOIN commande c ON r.nom_recette = c.nom_recette WHERE date_c > DATE_SUB(NOW(), INTERVAL 7 DAY) GROUP BY c.nom_recette ORDER BY count(c.nom_recette) DESC LIMIT 1 OFFSET " + i + "; ";
                            string desc = "SELECT r.descriptif_recette FROM recette r JOIN commande c ON r.nom_recette = c.nom_recette WHERE date_c > DATE_SUB(NOW(), INTERVAL 7 DAY) GROUP BY c.nom_recette ORDER BY count(c.nom_recette) DESC LIMIT 1 OFFSET " + i + "; ";
                            string prix = "SELECT r.prix_vente FROM recette r JOIN commande c ON r.nom_recette = c.nom_recette WHERE date_c > DATE_SUB(NOW(), INTERVAL 7 DAY) GROUP BY c.nom_recette ORDER BY count(c.nom_recette) DESC LIMIT 1 OFFSET " + i + "; ";
                            string nb_co = "SELECT count(c.nom_recette) FROM recette r JOIN commande c ON r.nom_recette = c.nom_recette WHERE date_c > DATE_SUB(NOW(), INTERVAL 7 DAY) GROUP BY c.nom_recette ORDER BY count(c.nom_recette) DESC LIMIT 1 OFFSET " + i + "; ";
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(co.Requete_lecture(nom));
                            Console.ResetColor();
                            Console.WriteLine(co.Requete_lecture(type) + " | Description : " + co.Requete_lecture(desc) + " | " + co.Requete_lecture(prix) + "cook | " + co.Requete_lecture(nb_co) + " commandes.");
                        }
                        Console.WriteLine();
                        Console.WriteLine("- Le CdR d'Or (CdR ayant été le plus commandé) : ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        string nom_cdr = "SELECT r.nom_cdr FROM recette r JOIN commande c ON r.nom_recette = c.nom_recette GROUP BY r.nom_cdr HAVING count(*) >= ALL(SELECT count(*) FROM commande GROUP BY nom_recette); ";
                        string str = co.Requete_lecture(nom_cdr);
                        Console.WriteLine(str);
                        Console.ResetColor();
                        Console.WriteLine();
                        Console.WriteLine("- Ses 5 meilleures recettes: ");
                        int best = int.Parse(co.Requete_lecture("SELECT count(nom_recette) FROM recette WHERE nom_cdr='"+str+"' AND nb_commande != 0 ORDER BY nb_commande DESC;"));
                        if(best>5)
                        {
                            best = 5;
                        }
                        for (int i=0;i<best;i++)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            CommandText = "SELECT c.nom_recette FROM commande c JOIN recette r ON c.nom_recette = r.nom_recette WHERE r.nom_cdr = '" + str + "' GROUP BY c.nom_recette ORDER BY count(c.nom_recette) DESC LIMIT 1 OFFSET " + i + "; ";
                            Console.Write(co.Requete_lecture(CommandText));
                            Console.ResetColor();
                            CommandText2 = "SELECT count(c.nom_recette) FROM commande c JOIN recette r ON c.nom_recette = r.nom_recette WHERE r.nom_cdr = '" + str + "' GROUP BY c.nom_recette ORDER BY count(c.nom_recette) DESC LIMIT 1 OFFSET "+i+"; ";
                            Console.WriteLine(" - " + co.Requete_lecture(CommandText2) + " commandes.");
                        }
                        Console.WriteLine();
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("--- Réapprovisionnement hebdomadaire des produits ---");
                        Console.ResetColor();
                        CommandText = "";
                        //Compte le nombre de produtis n'ayant pas été utilisé ces 30 derniers jours
                        int compte = Convert.ToInt32(co.Requete_lecture("SELECT count(nom_produit) FROM produit WHERE nom_produit IN(SELECT nom_produit FROM " +
                            "produit WHERE nom_produit NOT IN(SELECT p.nom_produit FROM produit p JOIN contient c ON p.nom_produit = c.nom_produit " +
                            "WHERE c.nom_recette IN(SELECT r.nom_recette FROM recette r JOIN commande c ON r.nom_recette = c.nom_recette WHERE date_c " +
                            "> DATE_SUB(NOW(), INTERVAL 30 DAY) GROUP BY c.nom_recette ORDER BY count(c.nom_recette) DESC)));"));
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("--Modification des stocks min et max pour cause d'inutilisation --");
                        Console.ResetColor();
                        //Iteration a travers chaque produit n'ayant pas été utilisé ces 30 derniers jours
                        for (int i = 0; i < compte; i++)
                        {
                            //Récupère le nom du produit à la position i
                            string prod = co.Requete_lecture(CommandText = "SELECT nom_produit FROM produit WHERE nom_produit IN " +
                                        "(SELECT nom_produit FROM produit WHERE nom_produit NOT IN (SELECT p.nom_produit FROM produit " +
                                        "p JOIN contient c ON p.nom_produit = c.nom_produit WHERE c.nom_recette IN(SELECT r.nom_recette " +
                                        "FROM recette r JOIN commande c ON r.nom_recette = c.nom_recette WHERE date_c > DATE_SUB(NOW(), " +
                                        "INTERVAL 30 DAY) GROUP BY c.nom_recette ORDER BY count(c.nom_recette) DESC))) LIMIT 1 OFFSET " + i + ";");
                            //Récupère le reste des informations du produit
                            Produit p1 = new Produit(prod, co);
                            p1.Modifier_stock_max((p1.Stock_max / 2), co);
                            p1.Modifier_stock_min((p1.Stock_min / 2), co);
                            Console.WriteLine("Le nouveau stock min de "+p1.Produit_nom+" est de " + p1.Stock_min + p1.Unite_quantite);
                            Console.WriteLine("Le nouveau stock max de " + p1.Produit_nom + " est de " + p1.Stock_max + p1.Unite_quantite);
                        }
                        Creation_XML(co);
                        

                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("-- Supprimer une recette --\n");
                        Console.ResetColor();
                        Console.WriteLine("Voici la liste des recettes :");
                        int nb_recette = int.Parse(co.Requete_lecture("SELECT count(*) FROM recette;"));
                        for (int i = 0; i < nb_recette; i++)
                        {
                            Console.Write(i + " - ");
                            Console.WriteLine(co.Requete_lecture("SELECT nom_recette FROM recette LIMIT 1 OFFSET " + i + ";"));
                        }
                        Console.WriteLine("\n Quelle recette souhaitez-vous supprimer ?");
                        int suppr = Convert.ToInt32(Console.ReadLine());
                        string nom_suppr = co.Requete_lecture("SELECT nom_recette FROM recette LIMIT 1 OFFSET " + suppr + ";");
                        Recette recette_suppr = new Recette(nom_suppr, co);
                        recette_suppr.Supprimer(co);
                        break;
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("--- Supprimer un CdR (et toute ses recettes) reste client ou non ---");
                        Console.ResetColor();
                        Console.WriteLine("Voici la liste des CdR :");
                        int nb_cdr = int.Parse(co.Requete_lecture("SELECT count(*) FROM createur_de_recettes;"));
                        for (int i = 0; i < nb_cdr; i++)
                        {
                            Console.Write(i + " - ");
                            Console.WriteLine(co.Requete_lecture("SELECT nom_cdr FROM createur_de_recettes LIMIT 1 OFFSET " + i + ";"));
                        }
                        Console.WriteLine("\n Quel CdR souhaitez-vous supprimer ?");
                        int suppr2 = Convert.ToInt32(Console.ReadLine());
                        string nom_suppr2 = co.Requete_lecture("SELECT nom_cdr FROM createur_de_recettes LIMIT 1 OFFSET " + suppr2 + ";");
                        Client client_suppr = new Client(nom_suppr2, co);
                        client_suppr.Modifier_cdr(false, co);
                        break;
                    case 5:
                        Console.WriteLine("Deconnexion...");
                        deco_Cook = true;
                        break;
                }
            } while (!deco_Cook);
        }
        /// <summary>
        /// Creation du bon de commande XML aux fournisseurs
        /// </summary>
        /// <param name="co"></param>
        static void Creation_XML(CoSQL co)
        {
            List<Produit> produit_com = new List<Produit> { };
            List<int> qte_com = new List<int> { };
            List<string> four_com = new List<string> { };
            //Création de la commande fournisseur
            // Parcourir le nombre de produits ayant le stock actuel inférieurau stock minimal
            for (int i = 0; i < Convert.ToInt32(co.Requete_lecture("SELECT count(*) from produit WHERE stock_actuel < stock_min ORDER BY nom_fournisseur; ")); i++) // 2
            {
                string prod = co.Requete_lecture("SELECT nom_produit from produit WHERE stock_actuel < stock_min ORDER BY nom_fournisseur LIMIT 1 OFFSET " + i + ";");
                Produit p1 = new Produit(prod, co);
                string four = co.Requete_lecture("SELECT nom_fournisseur from produit WHERE stock_actuel < stock_min ORDER BY nom_fournisseur LIMIT 1 OFFSET " + i + ";");
                int qte = p1.Stock_max - p1.Stock_actuel;
                produit_com.Add(p1);
                four_com.Add(four);
                qte_com.Add(qte);
            }
            
            if(produit_com.Count != 0)
            {
                XmlWriter xmlWriter = XmlWriter.Create("commande.xml");
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Commande");
                for (int j = 0; j < four_com.Count; j++)
                {
                    if (j == 0 || (j != 0 && four_com[j] != four_com[j - 1]))
                    {
                        xmlWriter.WriteStartElement("Fournisseur");
                        xmlWriter.WriteAttributeString("nom", four_com[j]);
                        for (int i = 0; i < produit_com.Count; i++)
                        {
                            if (produit_com[i].Fournisseur_nom == four_com[j])
                            {
                                xmlWriter.WriteStartElement("Produit");
                                xmlWriter.WriteAttributeString("REF", produit_com[i].Fournisseur_reference);
                                xmlWriter.WriteStartElement("qte");
                                xmlWriter.WriteString(Convert.ToString(qte_com[i]));
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteEndElement();
                            }
                        }
                        xmlWriter.WriteEndElement();
                    }
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
                System.Diagnostics.Process.Start("commande.xml");

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nAprès réapprovisionnement :");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("\nIl n'y a pas de produit à réapprovisionner.\n");
            }
            for(int i=0;i<produit_com.Count;i++)
            {
                Console.WriteLine("Le nouveau stock de "+produit_com[i].Produit_nom + " est de "+(produit_com[i].Stock_actuel+qte_com[i])+produit_com[i].Unite_quantite);
                produit_com[i].Modifier_stock_actuel(produit_com[i].Stock_actuel+qte_com[i],co);
            }            
        }
        /// <summary>
        /// Code principal - Execution
        /// </summary>
        /// <param name="co"></param>
        static void Execution(CoSQL co)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Bonjour et bienvenue !");
            Console.ResetColor();
            Console.WriteLine("Vous êtes :");
            Console.WriteLine("1 - Un client (ou créateur de recette)");
            Console.WriteLine("2 - Un employé de la société Cooking");
            Console.Write("Choix : ");
            int moi = Convert.ToInt32(Console.ReadLine());

            if (moi == 1) //Interface Client
            {
                Console.WriteLine("Avez-vous déjà un compte ? (Oui/Non) ");
                string compte = Convert.ToString(Console.ReadLine());
                compte = compte.ToUpper();
                if (compte == "OUI")
                {
                    Client c1 = Connexion(co);
                    if (c1.Client_createur) //Client CdR
                    {
                        c1 = Menu_CdR(c1, co);
                    }
                    else // Client non CdR
                    {
                        c1 = Menu_client(c1, co);
                    }
                }
                else
                {
                    Client c1 = Inscription(co);
                    if (c1.Client_createur == true)
                    {
                        Creation_recette(c1, co);
                        Menu_CdR(c1, co);
                    }
                    else
                    {
                        Menu_client(c1, co);
                    }
                }
            }
            else //Interface Cooking
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("=== Connexion ===");
                Console.ResetColor();
                Console.Write("Nom :");
                string nom = Convert.ToString(Console.ReadLine());
                while(co.Requete_lecture("SELECT count(*) FROM cooking WHERE nom_e = '" + nom + "';") != "1")
                {
                    Console.Write(nom +" n'est pas un employé de Cooking. Entrez un autre nom :");
                    nom = Convert.ToString(Console.ReadLine());
                }
                if (co.Requete_lecture("SELECT count(*) FROM cooking WHERE nom_e = '" + nom + "';") == "1")
                {
                    Console.Clear();
                    Menu_Cooking(co);
                }
            }
            Console.WriteLine("Aurevoir");
        }
        static void DEMO(CoSQL co)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("=== DEMO ===\n");
            Console.WriteLine("Voici le nombre de clients de notre application Cooking : ");
            Console.ResetColor();
            Console.WriteLine(co.Requete_lecture("SELECT count(*) FROM client;"));
            
            int nb_cdr = Convert.ToInt32(co.Requete_lecture("SELECT count(*) FROM createur_de_recettes;"));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nIl y a " + nb_cdr + " créateurs de recettes.");
            Console.ResetColor();
            for(int i=0;i<nb_cdr;i++)
            {
                string nom = co.Requete_lecture("SELECT nom_cdr FROM createur_de_recettes LIMIT 1 OFFSET " + i + ";");
                Console.WriteLine(nom + " - " + co.Requete_lecture("SELECT count(r.nom_cdr) FROM recette r JOIN commande c ON r.nom_recette = c.nom_recette WHERE r.nom_cdr = '"+nom+"';")+" commandes.");
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nVoici le nombre de recettes dans notre application Cooking :");
            Console.ResetColor();
            Console.WriteLine(co.Requete_lecture("SELECT count(*) FROM recette;"));

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nListe des produits ayant une quantité en stock <=2*minimale");
            Console.ResetColor();
            Console.WriteLine(co.Requete_lecture("SELECT nom_produit FROM produit WHERE stock_actuel<=2*stock_min;"));

            string pro = "";
            do
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nSaisir un produit pour afficher les informations : (Saisir non pour quitter)");
                Console.ResetColor();
                pro = Console.ReadLine();
                if(pro.ToUpper() != "NON")
                {

                }
                int nb = Convert.ToInt32(co.Requete_lecture("SELECT count(*) FROM contient WHERE nom_produit = '" + pro + "';"));
                for (int i = 0; i < nb; i++)
                {
                    string recette = co.Requete_lecture("SELECT nom_recette FROM contient WHERE nom_produit = '" + pro + "' LIMIT 1 OFFSET " + i+";");
                    Recette r1 = new Recette(recette, co);
                    Produit p1 = new Produit(pro, co);
                    string qte = co.Requete_lecture("SELECT quantite_p FROM contient WHERE nom_produit ='"+pro+"' LIMIT 1 OFFSET " + i + ";");
                    Console.WriteLine("La recette " + r1.Recette_nom + " contient " + qte + p1.Unite_quantite + " " + pro );
                }
                
            } while (pro.ToUpper() != "NON");
        }
        static void Main(string[] args)
        { 
            try
            {
                Console.WriteLine("### Entrez votre mot de passe de connexion à la bdd SQL ###");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("SERVER=127.0.0.1; DATABASE=cooking; UID=root; PASSWORD=");
                Console.ResetColor();
                string mdp = Console.ReadLine();
                CoSQL co = new CoSQL(mdp);
                bool fin = false;
                do
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("=== MENU GENERAL ===");
                    Console.ResetColor();
                    Console.WriteLine("Que voulez vous faire ? \n" +
                    "1 - Démarrer le mode de démo \n" +
                    "2 - Acceder à la plateforme Cooking \n");
                    Console.Write("Choix : ");
                    int choix = Convert.ToInt32(Console.ReadLine());
                    switch (choix)
                    {
                        case 1:
                            DEMO(co);
                            break;

                        case 2:
                            Console.Clear();
                            Execution(co);
                            fin = true;
                            break;
                    }
                } while (fin == false);
                

            }
            catch (MySqlException e)
            {
                Console.WriteLine(" ErreurConnexion : " + e.ToString());
                Console.ReadKey();
                return;
            }
        }
    }
}
