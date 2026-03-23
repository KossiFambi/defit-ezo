using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace calculatrice
{
    class Program
    {
        static void DemanderChaine()
        {
            string maChaine = "";
          
            while (maChaine == "")
            {
                Console.Write("Entrer une chaine de caractères :");
                maChaine = Console.ReadLine();
                              
                if (maChaine == "")
                {
                    Console.WriteLine("Erreur: la chaine ne doit pas etre vide");
                }
                else
                {
                    EvaluerChaine(maChaine);
                }
            }

        }
        

        static void EvaluerChaine(string p_chaine)
        {
            //supprimer les espaces vides
            p_chaine = p_chaine.Replace(" ", "");

            // Gérer les signes comme -- ou +- ou -+
            p_chaine = EvaluerOperateur(p_chaine);

            // definir un pattern pour les virgules flottantes
            string pattern = @"-?(?:\d+\.?\d*|\.\d+)";
           

            // 1. Traiter * et / => operateur prioritaire
            p_chaine = Calculer(p_chaine, $@"({pattern})([\*/])({pattern})");

            // 2. Traiter + et -
            p_chaine = Calculer(p_chaine, $@"({pattern})([+\-])({pattern})");

            Console.WriteLine("Le resultat est :" + p_chaine);

        }

        static string Calculer(string p_chaine, string pattern)
        {
            Regex regex = new Regex(pattern);

            while (regex.IsMatch(p_chaine))
            {
                p_chaine = regex.Replace(p_chaine, match =>
                {
                    double gauche = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    double droite = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                    string op = match.Groups[2].Value;

                    try
                    {

                        double result = op switch
                        {
                            "*" => gauche * droite,
                            "/" => droite == 0 ? throw new DivideByZeroException() : gauche / droite,
                            "+" => gauche + droite,
                            "-" => gauche - droite,
                            _ => throw new Exception("Opérateur inconnu")
                        };

                        return result.ToString(CultureInfo.InvariantCulture);
                    }
                    catch (DivideByZeroException)
                    {
                        Console.WriteLine("Erreur*");
                        return "";
                    }
                    
                }, 1); 
            }

            return p_chaine;
        }

        static string EvaluerOperateur(string p_chaine)
        {
            while (p_chaine.Contains("--") || p_chaine.Contains("+-") || 
                   p_chaine.Contains("-+") || p_chaine.Contains("++"))
            {
                p_chaine = p_chaine.Replace("--", "+")
                           .Replace("+-", "-")
                           .Replace("-+", "-")
                           .Replace("++", "+");
            }
           
            return p_chaine;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("-------------------------------------------------------------------------");
            Console.WriteLine("       Bienvenue sur l'interface d'évaluation de chaines de caratères    ");
            Console.WriteLine("--------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();
            bool continuer = true;

            while (continuer)
            {
                DemanderChaine();
  
                Console.Write("Voulez-vous continuer? (0/n)");
                string reponse = Console.ReadLine();
                if (reponse.ToLower() == "o")
                {
                    continuer = true;

                }
                else
                {
                    continuer = false;
                }

            }


        }
    }
}
