using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpotsLST
{
    enum CodeConjoint
    {
        Celi = 0,
        CNS = 1
    };

    class NombreDePartsNotFoundException:Exception
    {
        public NombreDePartsNotFoundException(string message) : base(message) { }
    }

    class GrossissementImpotAvantReductionParIntervalleException : Exception
    {
        public GrossissementImpotAvantReductionParIntervalleException(string message) : base(message) { }
    }

    class Employe
    {
        public string nom;
        public string prenom;
        public long  salaireBrut;
        public int nombreDeJours;
        public CodeConjoint conjoint;
        public int enfants;
        public string matricule;

        public Employe(string nom, string prenom, long  salaireBrut, 
            int nombreDeJours, CodeConjoint conjoint, int enfants, string matricule)
        {
            this.nom = nom;
            this.prenom = prenom;
            this.salaireBrut = salaireBrut;
            this.nombreDeJours = nombreDeJours;
            this.conjoint = conjoint;
            this.enfants = enfants;
            this.matricule = matricule;
        }


        public Employe()
        {

        }

        public long  salaireBrutAnnuel(long  salaireBrut, int  nombreDeJours)
        {
            return (360 * salaireBrut) / nombreDeJours;
        }

        public long  salaireBrutMensuel(long  salaireBrut, int nombreDeJours)
        {
            return (30 * salaireBrut) / nombreDeJours;
        }

        public long  abattement(long  salaireBrutAnnuel)
        {
            long  abat = (30 * salaireBrutAnnuel) / 100;
            return (abat > 900000) ? 900000 : abat; 
        }

        public long  brutFiscalApresAbattement(long  salaireBrutAnnuel)
        {
            return salaireBrutAnnuel - abattement(salaireBrutAnnuel);
        }

        public double nombreDeParts()
        {
            double nbPart = (this.enfants*0.5);
            if(this.conjoint == CodeConjoint.CNS)
            {
                nbPart += 1.5;
            }else if(this.conjoint == CodeConjoint.Celi)
            {
                nbPart += 1;
            }

            return nbPart;
        }


        public long [] pourcentageEtBornes()
        {
            long [] tab = new long [3];
            long  pourcentage; /* 20 = 20% */
            long  borneInf, borneSup;
            switch (this.nombreDeParts())
            {
                case 1.0:
                    pourcentage = 0;
                    borneInf = 0;
                    borneSup = 0;
                    break;
                case 1.5:
                    pourcentage = 10;
                    borneInf = 100000;
                    borneSup = 300000;
                    break;
                case 2:
                    pourcentage = 15;
                    borneInf = 200000;
                    borneSup = 650000;
                    break;
                case 2.5:
                    pourcentage = 20;
                    borneInf = 300000;
                    borneSup = 1100000;
                    break;
                case 3:
                    pourcentage = 25;
                    borneInf = 400000;
                    borneSup = 1650000;
                    break;
                case 3.5:
                    pourcentage = 30;
                    borneInf = 500000;
                    borneSup = 2030000;
                    break;
                case 4:
                    pourcentage = 35;
                    borneInf = 600000;
                    borneSup = 2490000;
                    break;
                case 4.5:
                    pourcentage = 40;
                    borneInf = 700000;
                    borneSup = 2755000;
                    break;
                case 5.0:
                    pourcentage = 45;
                    borneInf = 800000;
                    borneSup = 3180000;
                    break;
                default:
                    throw new NombreDePartsNotFoundException("Valeur de parts inattendue!");

            }
            tab[0] = pourcentage;
            tab[1] = borneInf;
            tab[2] = borneSup;
            return tab;
        }

        public long  reduction(long  impotAvantReduction)
        {
            long [] pourcentageEtBornes = this.pourcentageEtBornes();
            long  potentielReduction = (pourcentageEtBornes[0] * impotAvantReduction) /100;
            if(potentielReduction < pourcentageEtBornes[1])
            {
                return pourcentageEtBornes[1];
            }else if(potentielReduction > pourcentageEtBornes[2])
            {
                return pourcentageEtBornes[2];
            }else
            {
                return potentielReduction;
            }
        }

        /* On suppose que le salaireBrutAnnuel est positif */
        public long  impotAvantReduction(long  brutFiscalApresAbattement)
        {
            long  irppAvantReduction = 0;

            irppAvantReduction += grossissementImpotAvantReductionParIntervalle(brutFiscalApresAbattement, 0, 630000, 0);
            irppAvantReduction += grossissementImpotAvantReductionParIntervalle(brutFiscalApresAbattement, 630001, 1500000, 20);
            irppAvantReduction += grossissementImpotAvantReductionParIntervalle(brutFiscalApresAbattement, 1500001, 4000000, 30);
            irppAvantReduction += grossissementImpotAvantReductionParIntervalle(brutFiscalApresAbattement, 4000001, 8000000, 35);
            irppAvantReduction += grossissementImpotAvantReductionParIntervalle(brutFiscalApresAbattement, 8000001, 13500000, 37);
            irppAvantReduction += grossissementImpotAvantReductionParIntervalle(brutFiscalApresAbattement, 13500001, 1000000000, 40);

            return irppAvantReduction;
        }

        public long  grossissementImpotAvantReductionParIntervalle(long  brutFiscalApresAbattement, long  borneInf, long  borneSup, long  pourcentage /* 20 pour 20% */)
        {
            if(borneSup > borneInf && brutFiscalApresAbattement > 0 && pourcentage >=0 && pourcentage <=100)
            {
                if (brutFiscalApresAbattement >= borneInf && brutFiscalApresAbattement <= borneSup)
                {
                    return ((brutFiscalApresAbattement - borneInf + 1) * pourcentage) / 100;
                }
                else if (brutFiscalApresAbattement > borneSup)
                {
                    return ((borneSup - borneInf + 1) * pourcentage) / 100;
                }else
                {
                    return 0;
                }
            }

            throw new GrossissementImpotAvantReductionParIntervalleException("Les paramètres de la méthodes sont incohérentes!");
        }

        public long  impots(long  impotsAvantReduction, long  reduction)
        {
            return impotsAvantReduction - reduction;
        }

        public long  salaireNet(long  brutFiscalAnnuel, long  impots)
        {
            return brutFiscalAnnuel - impots;
        }


    }
}
