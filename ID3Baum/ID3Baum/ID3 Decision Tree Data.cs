using ID3Baum;
using System;

// Build the Decision Tree with this demo values and the ID3 Algorithm already learned
//https://en.wikipedia.org/wiki/ID3_algorithm
//https://www.programmingwithwolfgang.com/implementing-decision-tree-using-c-id3/
//https://data-science-blog.com/blog/2017/12/08/id3-algorithmus-ein-rechenbeispiel/

/// <summary>
/// These is the training data as enum jagged-Array
/// </summary>
Enum[][] data = new Enum[][] {
    new Enum[] { Outlook.Sunny, Temp.Hot, Humidity.High, Wind.Weak, Play.No },
    new Enum[] { Outlook.Sunny, Temp.Hot, Humidity.High, Wind.Strong, Play.No },
    new Enum[] { Outlook.Overcast, Temp.Hot, Humidity.High, Wind.Weak, Play.Yes },
    new Enum[] { Outlook.Rain, Temp.Mild, Humidity.High, Wind.Weak, Play.Yes },
    new Enum[] { Outlook.Rain, Temp.Cold, Humidity.Normal, Wind.Weak, Play.Yes },
    new Enum[] { Outlook.Rain, Temp.Cold, Humidity.Normal, Wind.Strong, Play.No },
    new Enum[] { Outlook.Overcast, Temp.Cold, Humidity.Normal, Wind.Weak, Play.Yes },
    new Enum[] { Outlook.Sunny, Temp.Mild, Humidity.High, Wind.Weak, Play.No },
    new Enum[] { Outlook.Sunny, Temp.Cold, Humidity.Normal, Wind.Weak, Play.Yes },
    new Enum[] { Outlook.Rain, Temp.Mild, Humidity.Normal, Wind.Strong, Play.Yes },
    new Enum[] { Outlook.Sunny, Temp.Mild, Humidity.Normal, Wind.Strong, Play.Yes },
    new Enum[] { Outlook.Overcast, Temp.Mild, Humidity.High, Wind.Strong, Play.Yes },
    new Enum[] { Outlook.Overcast, Temp.Hot, Humidity.Normal, Wind.Weak, Play.Yes },
    new Enum[] { Outlook.Rain, Temp.Mild, Humidity.High, Wind.Strong, Play.No }
};

DecisionTree decisionTree = new DecisionTree();
var rootNode = decisionTree.Train<Play>(data);
Play result = rootNode.Evaluate(new Enum[] { Outlook.Rain, Humidity.Normal, Wind.Strong });
Console.WriteLine("Result: " + result);
public class DecisionTree
{
    public Node<T> Train<T>(Enum[][] data)
    {
        Type labelType = typeof(T);
        //Getting the label
        Array labelDatas = Enum.GetValues(labelType);
        // Enum values as "Enum" example
        Array enumValues = Enum.GetValues(data[0][0].GetType());
        foreach (var enumItem in enumValues)
            Console.WriteLine(enumItem);

        //User gibt den Datentyp des Labels an
        //getting label index:
        int labelIndex = 0;
        for (int i = 0; i < data[0].Length; i++)
        {
            if (data[0][i].GetType() == labelType)
                labelIndex = i;
        }
        Console.WriteLine();
        Console.WriteLine("Label DataType: " + labelType);

        //Step 1: Getting the element with the highgest information gain.
        //Step 1.1: Calculating the total entropy of the dataset

        //Now calculating the entropy for each attribute: 


        return LearnData<T>(data);
        //Node<T> resultNode = new Node<T>(labelType, default(T));
        //resultNode.Result = labelDatas[i];
        //return resultNode;
    }
    private Node<T> LearnData<T>(Enum[][] baseData)
    {
        List<string> typesToIgnore = new();

        Type labelType = typeof(T);
        Node<T> rootNode = new Node<T>();


        //Getting the label
        Array labelDatas = Enum.GetValues(labelType);
        //Get local total entropy: 
        int[] labelValues = new int[labelDatas.Length]; //Values of the labels (How often does it appear?)

        Console.WriteLine("-----------------");
        for (int i = 0; i < baseData.Length; i++)
        {
            for (int j = 0; j < baseData[i].Length; j++)
            {
                if (baseData[i][j].GetType() == labelType)
                    labelValues[(int)((object)baseData[i][j])] += 1;
            }
        }
        List<int> labelValuesList = labelValues.ToList();
        double totalEntropy = DecisionMethods.Entropy(labelValuesList);
        Console.WriteLine("Local total entropy: " + totalEntropy);

        //The information gain of the label is double MaxValue to not get problems with the index (in case the label is in the middle of the dataset) 
        List<double> informationGains = new();
        var enumsInTable = baseData[0];
        foreach (var enumInTable in enumsInTable)
        {
            List<double> entropies = new(); //contains entropy value
            List<int> appearances = new(); // the times the value appears ("Si")
            var items = Enum.GetValues(enumInTable.GetType());
            for (int l = 0; l < items.Length; l++)
            {
                if (items.GetValue(l).GetType() != labelType) //preventing a loop over the tag and the current root node!
                {
                    int[] currentLabelValues = new int[labelDatas.Length]; //Values of the labels (How often does it appear?)
                    Console.WriteLine("-----------------");
                    Console.WriteLine("Enum: " + items.GetValue(l));
                    for (int i = 0; i < baseData.Length; i++)
                    {
                        for (int j = 0; j < baseData[i].Length; j++)
                        {
                            if ((int)((object)baseData[i][j]) == (int)((object)items.GetValue(l)) && items.GetValue(l).GetType() == baseData[i][j].GetType())
                            {
                                for (int k = 0; k < baseData[i].Length; k++)
                                {
                                    if (baseData[i][k].GetType() == labelType)
                                        currentLabelValues[(int)((object)baseData[i][k])] += 1;
                                }
                            }
                        }
                    }
                    List<int> currentLabelValuesList = currentLabelValues.ToList();
                    entropies.Add(DecisionMethods.Entropy(currentLabelValuesList));
                    appearances.Add(currentLabelValues.Sum());
                    Console.WriteLine(currentLabelValues[0]);
                    Console.WriteLine(currentLabelValues[1]);
                    Console.WriteLine("Entropie: " + entropies.Last());
                }
            }

            //calculate the information gain 
            if (entropies.Count > 0)
                informationGains.Add(DecisionMethods.Gain(totalEntropy, entropies, appearances)); //0 is a placeholder!
            else
                informationGains.Add(double.NaN);
        }
        Console.WriteLine("-----------------");


        //checking, which enums value has the highest information gain:
        if (informationGains.Count > 0)
        {
            Console.WriteLine("Information Gains: ");
            for (int i = 0; i < informationGains.Count; i++)
            {
                //Check if the information gain  is 0. If it is, this part of the tree has converged - the label is returned!
                if (informationGains[i] == 0) //Abruchbedingung
                {
                    Node<T> resultNode = new Node<T>(labelType, default(T));
                    foreach (var item in baseData[0])
                    {
                        if(item.GetType() == labelType)
                            resultNode.Result = (T)Enum.Parse(typeof(T), ((int)(object)item).ToString(), true);
                    }
                    return resultNode;
                }
                Console.WriteLine(baseData[0][i].GetType() + ": " + informationGains[i]);
            }
            Console.WriteLine("-----------------");

            int rootNodeIndex = informationGains.IndexOf(informationGains.Max()); //This is the index of the item for the next knot! (This index now becomes the root of the decision tree)
            Console.WriteLine("New root node is: " + baseData[0][rootNodeIndex].GetType());
            rootNode.AttrType = baseData[0][rootNodeIndex].GetType();
            //We now calculate the values for the subset: 
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(baseData[0][rootNodeIndex].GetType());
            var splittedData = SplitData(baseData, rootNodeIndex);//Splitting the dataset at the root node!
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < splittedData.Count; i++)
            {
                if (splittedData[i].Any() && !typesToIgnore.Contains(baseData[0][rootNodeIndex].GetType().GetEnumNames()[i])) //TODO: FILTER OUT LABEL
                {
                    rootNode.Children.Add(((Enum)Enum.GetValues(baseData[0][rootNodeIndex].GetType()).GetValue(i), LearnData<T>(splittedData[i])));
                }
            }
        }

        return rootNode;
    }

    /// <summary>
    /// Splits a given dataset by the specified index of the "root" datatype that will be used to split
    /// </summary>
    /// <param name="dataToSplit"></param>
    /// <param name="splitTypeIndex"></param>
    /// <returns></returns>
    List<Enum[][]> SplitData(Enum[][] dataToSplit, int splitTypeIndex)
    {
        Type splitType = dataToSplit[0][splitTypeIndex].GetType(); //The type to split byy 

        List<Enum[][]> splittedData = new(); //Placeholder to modify the dataarray 

        foreach (var splitTypeElement in Enum.GetValues(splitType)) //Looping through each value of the Enum we are splitting by 
        {
            Console.WriteLine(splitTypeElement);
            Enum[][] splittedDataPart = dataToSplit.Where(x => x[splitTypeIndex].Equals(splitTypeElement)).ToArray(); //Getting the part of the data that contains the enum value
            List<Enum[]> newSplittedDataPart = new(); //Placeholder  list that will later be converted to an array
            for (int i = 0; i < splittedDataPart.Length; i++) //Looping through each row of the splitted data to remove the element we are splitting by
            {
                List<Enum> splittedDataPartList = splittedDataPart[i].ToList(); //converting to a list to be able to modify it
                splittedDataPartList.RemoveAll(x => x.Equals(splitTypeElement)); //Removing the element we are splitting by from the row
                newSplittedDataPart.Add(splittedDataPartList.ToArray()); //Converting it back to an array to return it
            }

            splittedData.Add(newSplittedDataPart.ToArray());
        }
        return splittedData;
    }
}


// The enum datatypes to use for this sample
enum Outlook
{
    Sunny,
    Overcast,
    Rain
}
enum Humidity
{
    Low,
    Normal,
    High
}
enum Wind
{
    Weak,
    Strong
}
enum Temp
{
    Cold,
    Mild,
    Hot
}
enum Play
{
    Yes = 0,
    No = 1
}
