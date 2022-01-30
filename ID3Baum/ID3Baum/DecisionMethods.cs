using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3Baum
{
    internal static class DecisionMethods
    {

        public static double Entropy(List<int> numbers) 
        {
            double entropy = 0;

            for (int i = 0; i < numbers.Count; i++)
                    entropy += ((-(double)numbers[i] / (double)numbers.Sum()) * Math.Log2((double)numbers[i] / (double)numbers.Sum()));    

            if (double.IsNaN(entropy))
                entropy = 0;

            return entropy;
        }


        public static double Gain(double totalEntropy , List<double> entropies, List<int> appearances)
        {
            double informationGain = totalEntropy;
            for (int i = 0; i < entropies.Count; i++)
            {
                informationGain -= (((double)appearances[i] / (double)appearances.Sum()) * (double)entropies[i]);
            }

            return informationGain;
        }
    }
}
