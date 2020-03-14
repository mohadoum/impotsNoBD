using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace ImpotsLST
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SortedList<string, Employe> listEmployes = new SortedList<string, Employe>();
        public MainWindow()
        {
            InitializeComponent();
            gridListEmployes.Visibility = Visibility.Hidden;
           
          
            
        }


        private void boutonValider_Click(object sender, RoutedEventArgs e)
        {
            Employe emp;
            if(controleEntreesPourUnEmploye(nomEmploye.Text, prenomEmploye.Text, salaireBrutEmploye.Text, nombreJoursEmploye.Text, conjointEmploye.Text, enfantsEmploye.Text, matriculeEmploye.Text,out emp))
            {
                if(!listEmployes.ContainsKey(emp.matricule))
                {
                    if(emp.salaireBrut >= 50000)
                    { 
                        if(emp.salaireBrutAnnuel(emp.salaireBrut, emp.nombreDeJours) >0)
                        {
                            if (emp.nombreDeParts() <= 5.0)
                            { 
                                listEmployes.Add(emp.matricule, emp);
                                long  sba = emp.salaireBrutAnnuel(emp.salaireBrut, emp.nombreDeJours);
                                long  abat = emp.abattement(sba);
                                long  bfaa = emp.brutFiscalApresAbattement(sba);
                                double parts = emp.nombreDeParts();
                                long  irppar = emp.impotAvantReduction(bfaa);
                                long  reduc = emp.reduction(irppar);
                                long  imp = emp.impots(irppar, reduc);
                                long  salNet = emp.salaireNet(sba, imp);

                                brutFiscalAnnuel.Content = sba;
                                abattement.Content = abat;
                                brutFiscalApresAbattement.Content = bfaa;
                                nombreParts.Content = parts;
                                irppAvantReduction.Content = irppar;
                                reduction.Content = reduc;
                                impots.Content = imp;
                                salaireNet.Content = salNet;

                                MessageBox.Show("Employé ajouté avec succés!");
                            }
                            else
                            {
                                MessageBox.Show("Le simulateur ne gère pas les employés avec des parts supérieur à 5.0!");
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Un salaire ne peut être aussi grand!");
                        }
                        

                        
                    }else
                    {
                        MessageBox.Show("Le salaire de l'employé ne peut être inférieur au SMIG!");
                        return;
                    }
                    

                }else
                {
                    MessageBox.Show("Ce matricule est déjà détenu par un autre employé!");
                    return;
                }
            }
        }

        private bool controleEntreesPourUnEmploye(string nom, string prenom, string salaireBrut, string nombreJour, string conjoint, string enfants, string matricule,out Employe emp)
        {
            emp = new Employe();

            /* Controle de champs non vide */

            nom = nom.Trim();
            prenom = prenom.Trim();
            salaireBrut = salaireBrut.Trim();
            nombreJour = nombreJour.Trim();
            conjoint = conjoint.Trim();
            enfants = enfants.Trim();
            matricule = matricule.Trim();

            if(nom == String.Empty)
            {
                MessageBox.Show("Veuillez renseigner le nom de l'employé!");
                return false;
            }

            if (prenom == String.Empty)
            {
                MessageBox.Show("Veuillez renseigner le prénom de l'employé!");
                return false;
            }

            if (salaireBrut == String.Empty)
            {
                MessageBox.Show("Veuillez renseigner le salaire Brût de l'employé!");
                return false;
            }

            if (nombreJour == String.Empty)
            {
                MessageBox.Show("Veuillez renseigner le nom de jours correspondant au salaire brût de l'employé!");
                return false;
            }

            if (conjoint == String.Empty)
            {
                MessageBox.Show("Veuillez renseigner le code corresponant au conjoint de l'employé!");
                return false;
            }

            if (enfants == String.Empty)
            {
                MessageBox.Show("Veuillez renseigner le nombre d'enfants de l'employé!");
                return false;
            }

            if (matricule == String.Empty)
            {
                MessageBox.Show("Veuillez renseigner le matricule de l'employé!");
                return false;
            }





            /* Controle chaine avec lettre alphabet */
            if (!checkNom(nom))
            {
                MessageBox.Show("Le nom de l'employé ne doit contenir ni chiffre, ni accent, ni caractère spécial!");
                return false;
            }

            if (!checkPrenom(prenom)) 
            {
                MessageBox.Show("Le prénom de l'employé ne doit contenir ni chiffre, ni accent, ni caractère spécial!");
                return false;
            }

            
            /* Controles valeurs entiers et speciaux */

            if(!long .TryParse(salaireBrut, out emp.salaireBrut))
            {
                MessageBox.Show("Veuillez rensigner un salaire Brut normal!");
                return false;
            }

            if(emp.salaireBrut<=0)
            {
                MessageBox.Show("Le salaire Brut de l'employé doit être supérieur à zéro!");
                return false;
            }

            if (!int.TryParse(nombreJour, out emp.nombreDeJours))
            {
                MessageBox.Show("Veuillez rensigner un nombre de jours valide!");
                return false;
            }

            if (emp.nombreDeJours <= 0)
            {
                MessageBox.Show("Le nombre de jours correspondant au salaire brût de l'employé doit être supérieur à zéro!");
                return false;
            }

            if(!(emp.nombreDeJours%30 == 0))
            {
                MessageBox.Show("Le nombre de jours correspondant au salaire brût de l'employé doit être un multiple de 30!");
                return false;
            }

            if (emp.nombreDeJours > 360)
            {
                MessageBox.Show("Le nombre de jours correspondant au salaire brût de l'employé doit être inférieur ou égale à 360 jours!");
                return false;
            }


            int  entier;
            if (!int.TryParse(conjoint, out entier))
            {
                MessageBox.Show("Le champs Conjoint n'accepte que les valeurs 0 (pour célibataire) ou 1 (pour conjoint non salarié)!");
                return false;
            }

            if (entier != 0 && entier != 1)
            {
                MessageBox.Show("Le champs Conjoint n'accepte que les valeurs 0 (pour célibataire) ou 1 (pour conjoint non salarié)!");
                return false;
            }

            if (!int.TryParse(enfants, out emp.enfants))
            {
                MessageBox.Show("Veuillez rensigner un nombre d'enfants valide!");
                return false;
            }

            if (emp.enfants < 0)
            {
                MessageBox.Show("Veuillez rensigner un nombre d'enfants valide!");
                return false;
            }


            /* Controle matricule */
            if(!checkMatricule(matricule))
            {
                MessageBox.Show("Le matricule doit se composer le chiffres ou lettres de l'alphabet!");
                return false;
            }


            emp = new Employe(nom, prenom, emp.salaireBrut, emp.nombreDeJours, (entier == 0)?CodeConjoint.Celi:CodeConjoint.CNS, emp.enfants, matricule);
            return true;
        }


        private bool checkNom(string chaine)
        {
            for (int i = 0; i < chaine.Length; i++)
            {
                char car = chaine[i];
                if (!(car >= 'A' && car <= 'Z') && !(car >= 'a' && car <= 'z'))
                {
                    return false;
                }
            }

            return true;
        }

        private bool checkPrenom(string chaine)
        {
            for (int i = 0; i < chaine.Length; i++)
            {
                char car = chaine[i];
                if (!(car >= 'A' && car <= 'Z') && !(car >= 'a' && car <= 'z') && !(car == ' '))
                {
                    return false;
                }
            }

            return true;
        }

        private bool checkMatricule(string chaine)
        {
            for (int i = 0; i < chaine.Length; i++)
            {
                char car = chaine[i];
                if (!(car >= 'A' && car <= 'Z') && !(car >= 'a' && car <= 'z') && !(car >= '0' && car <= '9'))
                {
                    return false;
                }
            }

            return true;
        }

        private void boutonListEmployes_Click(object sender, RoutedEventArgs e)
        {
            gridListEmployes.Visibility = Visibility.Visible;

            DataTable dt = new DataTable();

            DataColumn matricule = new DataColumn("Matricule", typeof(string));
            DataColumn nom = new DataColumn("Nom", typeof(string));
            DataColumn prenom = new DataColumn("Prenom", typeof(string));
            DataColumn conjoint = new DataColumn("Conjoint", typeof(int));
            DataColumn enfants = new DataColumn("Enfants", typeof(int));
            DataColumn bfa = new DataColumn("Brut Fiscal Annuel (BFA)", typeof(string));
            DataColumn abattement = new DataColumn("Abattement", typeof(string));
            DataColumn bfaa = new DataColumn("BFA Aprés Abattement", typeof(string));
            DataColumn irpp = new DataColumn("IRPP Avant Réduction", typeof(string));
            DataColumn parts = new DataColumn("Parts", typeof(string));
            DataColumn reduction = new DataColumn("Réduction", typeof(string));
            DataColumn impot = new DataColumn("Impot", typeof(string));
            DataColumn salaireNet = new DataColumn("SalaireNet", typeof(string));

            dt.Columns.Add(matricule);
            dt.Columns.Add(nom);
            dt.Columns.Add(prenom);
            dt.Columns.Add(conjoint);
            dt.Columns.Add(enfants);
            dt.Columns.Add(bfa);
            dt.Columns.Add(abattement);
            dt.Columns.Add(bfaa);
            dt.Columns.Add(irpp);
            dt.Columns.Add(parts);
            dt.Columns.Add(reduction);
            dt.Columns.Add(impot);
            dt.Columns.Add(salaireNet);




            IEnumerator<Employe> enu = listEmployes.Values.GetEnumerator();
            while(enu.MoveNext())
            {
                Employe emp = enu.Current;
                DataRow row = dt.NewRow();
                long  sba = emp.salaireBrutAnnuel(emp.salaireBrut, emp.nombreDeJours);
                long  abat = emp.abattement(sba);
                long  bfaa1 = emp.brutFiscalApresAbattement(sba);
                double parts1 = emp.nombreDeParts();
                long  irppar = emp.impotAvantReduction(bfaa1);
                long  reduc = emp.reduction(irppar);
                long  imp = emp.impots(irppar, reduc);
                long  salNet = emp.salaireNet(sba, imp);

                row[0] = emp.matricule;
                row[1] = emp.nom;
                row[2] = emp.prenom;
                row[3] = (int)emp.conjoint;
                row[4] = emp.enfants;
                row[5] = sba;
                row[6] = abat;
                row[7] = bfaa1;
                row[8] = irppar;
                row[9] = parts1;
                row[10] = reduc;
                row[11] = imp;
                row[12] = salNet;


                dt.Rows.Add(row);
            }
            
            dataGridEmployes.ItemsSource = dt.DefaultView;

        }

        private void boutonRetourAcceuilTop_Click(object sender, RoutedEventArgs e)
        {
            gridListEmployes.Visibility = Visibility.Hidden;
        }

        private void boutonRetourAcceuilBottom_Click(object sender, RoutedEventArgs e)
        {
            gridListEmployes.Visibility = Visibility.Hidden;
        }

        private void quitter_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void annuler_Click(object sender, RoutedEventArgs e)
        {
            nomEmploye.Text = "";
            prenomEmploye.Text = "";
            salaireBrutEmploye.Text = "";
            nombreJoursEmploye.Text = "30";
            conjointEmploye.Text = "0";
            enfantsEmploye.Text = "0";
            matriculeEmploye.Text = "";

            brutFiscalAnnuel.Content = "";
            brutFiscalApresAbattement.Content = "";
            irppAvantReduction.Content = "";
            abattement.Content = "";
            nombreParts.Content = "";
            reduction.Content = "";
            impots.Content = "";
            salaireNet.Content = "";

        }
    }
}
