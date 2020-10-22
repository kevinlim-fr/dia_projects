-- INT DECIMAL(M,N) VARCHAR() DATE TIMESTAMP
-- Génération de la Base et des tables
drop database Cooking;
create database Cooking;
use Cooking;

CREATE TABLE createur_de_recettes (
nom_cdr VARCHAR(20) NOT NULL, 
-- nom_client VARCHAR(20) NOT NULL, 
-- nom_e VARCHAR(20) NOT NULL, 
solde_cdr VARCHAR(10) NOT NULL,
PRIMARY KEY(nom_cdr));

INSERT INTO createur_de_recettes VALUES ('Mario', '100');
INSERT INTO createur_de_recettes VALUES ('Luc', '100');

CREATE TABLE client (
nom_client VARCHAR(20) NOT NULL, 
tel_client VARCHAR(10) NOT NULL, 
adresse_client VARCHAR(50) NOT NULL, 
nom_cdr VARCHAR(20), 
PRIMARY KEY(nom_client),
CONSTRAINT FK_client_nom_cdr FOREIGN KEY (nom_cdr) REFERENCES createur_de_recettes (nom_cdr));

INSERT INTO client (nom_client, tel_client, adresse_client, nom_cdr) VALUES ('Mario', '0601020301', '1 Rue Italie 75001 Paris','Mario');
INSERT INTO client (nom_client, tel_client, adresse_client, nom_cdr) VALUES ('Luc', '0601080301', '2 Rue de Paris 75001 Paris','Luc');
INSERT INTO client (nom_client, tel_client, adresse_client) VALUES ('Isabelle', '0606020302', '1 Rue de la Paix 75001 Paris');
INSERT INTO client (nom_client, tel_client, adresse_client) VALUES ('Bernard', '0606020306', '2 Rue de la Guerre 75001 Paris');
INSERT INTO client (nom_client, tel_client, adresse_client) VALUES ('Claude', '0605020306', '3 Rue de Paris 78000 Rambouillet');
INSERT INTO client (nom_client, tel_client, adresse_client) VALUES ('Michel', '0605050306', '4 Rue de la Paix 75001 Paris');

CREATE TABLE recette (
nom_recette VARCHAR(20), 
type_recette VARCHAR(20),  
descriptif_recette VARCHAR(256), 
prix_vente INT, 
nom_cdr VARCHAR(20),
nb_commande INT,
PRIMARY KEY (nom_recette),
CONSTRAINT FK_produit FOREIGN KEY (nom_cdr) REFERENCES createur_de_recettes (nom_cdr));

-- Kevin
INSERT INTO recette VALUES ('Ratatouille', 'Plat', 'Plat Méditéranéen de Luc','2','Luc',4);
INSERT INTO recette VALUES ('Salade César', 'Entree', 'Salade à base de poulet et sauce César','2','Luc',3);
INSERT INTO recette VALUES ('Tartes aux pommes', 'Dessert', 'Dessert a base de pommes','1','Luc',3);
INSERT INTO recette VALUES ('Tiramisu', 'Dessert', 'Dessert a base de Mascarpone','1','Mario',10);
INSERT INTO recette VALUES ('Lasagne Légumes', 'Plat', 'Dessert a base de pommes','1','Mario',2);
INSERT INTO recette VALUES ('Lasagne Bolognaise', 'Plat', 'Dessert a base de pommes','1','Mario',2);
INSERT INTO recette VALUES ('Pizza aux Truffes', 'Plat', 'Pizza','4','Mario',0);
INSERT INTO recette VALUES ('Spaghetti Carbonara', 'Plat', 'Plat Italien de Mario','3','Mario',2);
INSERT INTO recette VALUES ('Spaghetti Pesto', 'Plat', 'Plat Italien de Mario','3','Mario',0);
INSERT INTO recette VALUES ('Spaghetti Bolognaise', 'Plat', 'Plat Italien de Mario','3','Mario',5);
INSERT INTO recette VALUES ('Risotto','Plat','Riz italien','8','Mario',0);

CREATE TABLE commande (
nom_recette VARCHAR(20), FOREIGN KEY (nom_recette) REFERENCES recette(nom_recette), 
qte_recette INT,
nom_client VARCHAR(20), FOREIGN KEY(nom_client) REFERENCES client(nom_client),
num_commande INT, 
date_c DATE,
CONSTRAINT PK_commande PRIMARY KEY(nom_recette,nom_client,num_commande));

INSERT INTO commande VALUES ('Ratatouille',1,'Bernard',4,'2020-05-04');
INSERT INTO commande VALUES ('Ratatouille',1,'Claude',5,'2020-05-04');
INSERT INTO commande VALUES ('Spaghetti Carbonara',1,'Isabelle',6,'2020-05-04');
-- CDR d'OR MARIO - TOP 5: Spaghetti Bolo / Carbo / Tiramisu / Lasagne Bolo / Lasagne Légumes
INSERT INTO commande VALUES ('Spaghetti Bolognaise',1,'Bernard',1,'2020-03-01');
INSERT INTO commande VALUES ('Tiramisu',1,'Bernard',2,'2020-03-01');
INSERT INTO commande VALUES ('Spaghetti Carbonara',1,'Bernard',3,'2020-03-02');
INSERT INTO commande VALUES ('Tiramisu',1,'Bernard',4,'2020-03-02');
INSERT INTO commande VALUES ('Spaghetti Bolognaise',1,'Bernard',5,'2020-03-03');
INSERT INTO commande VALUES ('Tiramisu',1,'Bernard',6,'2020-03-03');
INSERT INTO commande VALUES ('Lasagne Bolognaise',1,'Bernard',7,'2020-03-04');
INSERT INTO commande VALUES ('Tiramisu',1,'Bernard',8,'2020-03-04');
INSERT INTO commande VALUES ('Lasagne Bolognaise',1,'Bernard',9,'2020-03-05');
INSERT INTO commande VALUES ('Tiramisu',1,'Bernard',10,'2020-03-05');
INSERT INTO commande VALUES ('Lasagne Légumes',1,'Bernard',11,'2020-03-06');
INSERT INTO commande VALUES ('Tiramisu',1,'Bernard',12,'2020-03-06');
INSERT INTO commande VALUES ('Lasagne Légumes',1,'Bernard',13,'2020-03-06');
INSERT INTO commande VALUES ('Tiramisu',1,'Bernard',14,'2020-03-06');
-- CDR SEMAINE Luc Top 5 des recettes de la semaine Ratatouille / Salade César / Tartes aux pommes / Spaghetti Bolognaise / Tiramisu
INSERT INTO commande VALUES ('Ratatouille',1,'Isabelle',15,'2020-05-08');
INSERT INTO commande VALUES ('Ratatouille',1,'Isabelle',16,'2020-05-08');
INSERT INTO commande VALUES ('Ratatouille',1,'Isabelle',17,'2020-05-08');
INSERT INTO commande VALUES ('Salade César',1,'Isabelle',18,'2020-05-08');
INSERT INTO commande VALUES ('Salade César',1,'Isabelle',19,'2020-05-08');
INSERT INTO commande VALUES ('Salade César',1,'Isabelle',20,'2020-05-08');
INSERT INTO commande VALUES ('Tartes aux pommes',1,'Isabelle',21,'2020-05-08');
INSERT INTO commande VALUES ('Tartes aux pommes',1,'Isabelle',22,'2020-05-08');
INSERT INTO commande VALUES ('Tartes aux pommes',1,'Isabelle',23,'2020-05-08');
INSERT INTO commande VALUES ('Spaghetti Bolognaise',1,'Isabelle',24,'2020-05-08');
INSERT INTO commande VALUES ('Spaghetti Bolognaise',1,'Isabelle',25,'2020-05-08');
INSERT INTO commande VALUES ('Spaghetti Bolognaise',1,'Isabelle',26,'2020-05-08');
INSERT INTO commande VALUES ('Tiramisu',1,'Isabelle',27,'2020-05-08');
INSERT INTO commande VALUES ('Tiramisu',1,'Isabelle',28,'2020-05-08');
INSERT INTO commande VALUES ('Tiramisu',1,'Isabelle',29,'2020-05-08');

CREATE TABLE fournisseur (
nom_f VARCHAR(20), 
tel_f VARCHAR(10), 
PRIMARY KEY(nom_f));

INSERT INTO fournisseur VALUES ('Barilla', '0102030102');
INSERT INTO fournisseur VALUES('Auchan', '0104030102');
INSERT INTO fournisseur VALUES('Ferme de la vallée', '0134039198');
INSERT INTO fournisseur VALUES('BioCbon', '0164390274');


CREATE TABLE produit (
nom_produit VARCHAR(20), 
categorie_p VARCHAR(20), 
unite_quantite VARCHAR(5), 
stock_actuel INT, 
stock_min INT, 
stock_max INT,
nom_fournisseur VARCHAR(20), 
ref_fournisseur VARCHAR(20), 
PRIMARY KEY(nom_produit),
CONSTRAINT FK_nom_fournisseur FOREIGN KEY (nom_fournisseur) REFERENCES fournisseur (nom_f));
-- Kevin
INSERT INTO produit VALUES ('Pâtes', 'Féculent', 'kg','50','10','100','Barilla','PAT');
INSERT INTO produit VALUES ('Sauce Bolognaise', 'Sauce', 'kg','50','10','100','Barilla','BOL');
INSERT INTO produit VALUES ('Sauce Carbonara', 'Sauce', 'kg','50','10','100','Barilla','CAR');
INSERT INTO produit VALUES ('Légumes', 'Légumes', 'pce','05','10','100','Ferme de la vallée','LEG');
INSERT INTO produit VALUES ('Riz', 'Féculent', 'kg','0','10','100','Auchan','RIZ');
INSERT INTO produit VALUES ('Patates','Féculent', 'kg','10','10','100','Ferme de la vallée','POE');
INSERT INTO produit VALUES ('Salade','Plantes','unite','10','10','100','Ferme de la vallée','SAL');
INSERT INTO produit VALUES ('Sauce César', 'Sauce', 'kg','10','10','100','Auchan','CES');
INSERT INTO produit VALUES ('Pâte à tarte', 'Pâte', 'unite','10','10','100','Auchan','TAR');
INSERT INTO produit VALUES ('Pommes', 'Fruit', 'kg','10','10','100','Ferme de la vallée','POM');
INSERT INTO produit VALUES ('Tiramisu', 'Dessert', 'Pot','10','10','100','Auchan','TIR');
INSERT INTO produit VALUES ('Pâte à lasagne', 'Féculent', 'kg','10','10','100','Barilla','LAS');
INSERT INTO produit VALUES ('Pâte à pizza', 'Pâte', 'kg','10','10','100','Barilla','PIZ');
INSERT INTO produit VALUES ('Truffes', 'Champignon', 'kg','10','10','100','Auchan','TRU');
INSERT INTO produit VALUES ('Sauce Pesto', 'Sauce', 'kg','10','10','100','Barilla','PES');

CREATE TABLE contient (
nom_recette VARCHAR(20), FOREIGN KEY(nom_recette) REFERENCES recette(nom_recette), 
nom_produit VARCHAR(20), FOREIGN KEY(nom_produit) REFERENCES produit(nom_produit), 
quantite_p INT,
CONSTRAINT PK_continent PRIMARY KEY(nom_recette,nom_produit));

INSERT INTO contient VALUES('Spaghetti Bolognaise','Pâtes', 1);
INSERT INTO contient VALUES('Spaghetti Bolognaise','Sauce Bolognaise', 1);
INSERT INTO contient VALUES('Spaghetti Carbonara','Pâtes', 1);
INSERT INTO contient VALUES('Spaghetti Carbonara','Sauce Carbonara', 1);
INSERT INTO contient VALUES('Ratatouille','Légumes', 1);
INSERT INTO contient VALUES('Salade César','Salade', 1);
INSERT INTO contient VALUES('Salade César','Sauce César', 1);
INSERT INTO contient VALUES('Tartes aux pommes','Pâte à tarte', 1);
INSERT INTO contient VALUES('Tartes aux pommes','Pommes', 1);
INSERT INTO contient VALUES('Lasagne Légumes','Légumes', 1);
INSERT INTO contient VALUES('Lasagne Légumes','Pâte à lasagne', 1);
INSERT INTO contient VALUES('Lasagne Bolognaise','Sauce Bolognaise', 1);
INSERT INTO contient VALUES('Lasagne Bolognaise','Pâte à lasagne', 1);
INSERT INTO contient VALUES('Pizza aux Truffes','Truffes', 1);
INSERT INTO contient VALUES('Pizza aux Truffes','Pâte à pizza', 1);
INSERT INTO contient VALUES('Spaghetti Pesto','Sauce Pesto', 1);
INSERT INTO contient VALUES('Spaghetti Pesto','Pâtes', 1);
INSERT INTO contient VALUES('Tiramisu','Tiramisu',1);
INSERT INTO contient VALUES('Risotto','Riz',1);
INSERT INTO contient VALUES('Risotto','Truffes',1);



CREATE TABLE cooking (
nom_e VARCHAR(20), 
adresse_e VARCHAR(50),
tel_e VARCHAR(10),
PRIMARY KEY(nom_e));

INSERT INTO cooking.cooking VALUES ('Alfred', '1 Rue de la Cuisine 75001 Paris', '0102030708');
INSERT INTO cooking.cooking VALUES ('Yan', '22 Rue de la Gare 78000 Versailles', '0123456789');
INSERT INTO cooking.cooking VALUES ('Kevin', '54 Avenue de la République 91645 Bussy', '0186943856');