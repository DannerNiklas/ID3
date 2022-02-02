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

//Testing the Decision tree
DecisionTree decisionTree = new DecisionTree();
var rootNode = decisionTree.Train<Play>(data);
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\nFinsihed the tree");
Console.WriteLine("-----------------");
Enum[] data_to_evaluate = new Enum[] { Outlook.Sunny, Temp.Cold, Humidity.High };
Play result = rootNode.Evaluate(data_to_evaluate);
Console.WriteLine("Data:");
foreach (var value in data_to_evaluate)
    Console.Write(value + " ");
Console.WriteLine("\nResult: " + result);

Console.ForegroundColor = ConsoleColor.White;

public class DecisionTree
{
    /// <summary>
    /// Train an ID3 Decision Tree based on a given dataset and the labeltype T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public Node<T> Train<T>(Enum[][] data)
    {
        Type labelType = typeof(T);

        //User gibt den Datentyp des Labels an
        //getting label index:
        int labelIndex = 0;
        for (int i = 0; i < data[0].Length; i++)
            if (data[0][i].GetType() == labelType)
                labelIndex = i;
        Console.WriteLine();
        Console.WriteLine("Label DataType: " + labelType);

        //Step 1: Getting the element with the highgest information gain.


        //Now calculating the entropy for each attribute: 
        return LearnData<T>(data);
    }
    /// <summary>
    /// Internal Function used that is called recursively, used to build the ID3 Tree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="baseData"></param>
    /// <returns></returns>
    private Node<T> LearnData<T>(Enum[][] baseData)
    {
        Type labelType = typeof(T);
        Node<T> rootNode = new Node<T>();

        //Getting the labels values (For example YES or NO)
        Array labelDatas = Enum.GetValues(labelType);
        //Get local total entropy: 
        int[] labelValues = new int[labelDatas.Length]; //Values of the labels (How often does it appear?). Used to calculate the entropy

        Console.WriteLine("-----------------");
        //The first step is to calculate the local total entropy of the dataset to later be able to calculate the information gain:
        //Step 1.1: Calculating the total entropy of the dataset
        for (int i = 0; i < baseData.Length; i++) //Looping through the dataset and counting each appearance of the labels values. E.g. that the labels "YES" value appears 3 times and the labels "NO" value appears 6 times.
            for (int j = 0; j < baseData[i].Length; j++)
                if (baseData[i][j].GetType() == labelType)
                    labelValues[(int)((object)baseData[i][j])] += 1;

        List<int> labelValuesList = labelValues.ToList(); //Converting it to a list to pass it to the entropy function
        double totalEntropy = DecisionMethods.Entropy(labelValuesList);
        Console.WriteLine("Local total entropy: " + totalEntropy);

        //The information gain of the label is set to NaN to not get problems with the index (in case the label is in the middle of the dataset). 
        List<double> informationGains = new();
        var enumsInTable = baseData[0]; //THe first row of the dataset 
        foreach (var enumInTable in enumsInTable) //looping through every Enum Type of the current dataset
        {
            List<double> entropies = new(); //contains entropy value
            List<int> appearances = new(); // the times the value appears ("Si")
            var items = Enum.GetValues(enumInTable.GetType());
            for (int l = 0; l < items.Length; l++) //Looping over each value of the enum type
            {
                if (items.GetValue(l).GetType() != labelType) //preventing a loop over the tag and the current root node!
                {
                    int[] currentLabelValues = new int[labelDatas.Length]; //Values of the labels (How often does it appear?)
                    Console.WriteLine("-----------------");
                    Console.WriteLine("Enum: " + items.GetValue(l));
                    for (int i = 0; i < baseData.Length; i++) //Now looping over the entire current dataset again to count the appearances of the current type
                        for (int j = 0; j < baseData[i].Length; j++) //each row of the current dataset
                            if ((int)((object)baseData[i][j]) == (int)((object)items.GetValue(l)) && items.GetValue(l).GetType() == baseData[i][j].GetType()) //Checking if it's the same enum value/element
                                for (int k = 0; k < baseData[i].Length; k++) //Now looping through to count the label appearnace 
                                    if (baseData[i][k].GetType() == labelType)
                                        currentLabelValues[(int)((object)baseData[i][k])] += 1;
                    //Now the local entropy of the according enum value is calculated
                    List<int> currentLabelValuesList = currentLabelValues.ToList();
                    entropies.Add(DecisionMethods.Entropy(currentLabelValuesList));
                    appearances.Add(currentLabelValues.Sum()); //Counting the appearances for the info gain
                    Console.WriteLine(currentLabelValues[0]);
                    Console.WriteLine(currentLabelValues[1]);
                    Console.WriteLine("Entropie: " + entropies.Last());
                }
            }

            //calculate the information gain 
            if (entropies.Count > 0)
                informationGains.Add(DecisionMethods.Gain(totalEntropy, entropies, appearances)); //0 is a placeholder!
            else
                informationGains.Add(double.NaN); //to prevent wrong indexing
        }
        Console.WriteLine("-----------------");


        //checking, which enums value has the highest information gain:
        if (informationGains.Count > 0) //Making sure there are still info gains that have been calculated
        {
            Console.WriteLine("Information Gains: ");
            for (int i = 0; i < informationGains.Count; i++) //Checking each information gain to see if a distinct label has been found yet
            {
                //Check if the information gain  is 0. If it is, this part of the tree has converged - the label is returned!
                if (informationGains[i] == 0) //Abruchbedingung
                {
                    Node<T> resultNode = new Node<T>(labelType, default(T)); //The node that will be returned
                    foreach (var item in baseData[0]) //adding the label value to it (itering over the first row of the current dataset to get the value. As the value is distinct in the current dataset this is okay
                        if (item.GetType() == labelType)
                            resultNode.Result = (T)Enum.Parse(typeof(T), ((int)(object)item).ToString(), true);
                    return resultNode;
                }
                Console.WriteLine(baseData[0][i].GetType() + ": " + informationGains[i]);
            }
            Console.WriteLine("-----------------");

            int rootNodeIndex = informationGains.IndexOf(informationGains.Max()); //This is the index of the item for the next knot! (This index now becomes the root of the decision tree)
            Console.WriteLine("New root node is: " + baseData[0][rootNodeIndex].GetType());
            rootNode.AttrType = baseData[0][rootNodeIndex].GetType(); //Setting the Attribute Type of the current node
            //We now calculate the values for the subset: 
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(baseData[0][rootNodeIndex].GetType());
            var splittedData = SplitData(baseData, rootNodeIndex);//Splitting the dataset at the root node!
            Console.ForegroundColor = ConsoleColor.White;
            //Iterating over the splitted dataset of the root node
            for (int i = 0; i < splittedData.Count; i++) //Finally, every dataset that doesn't have a distinct answer yet is iterated over recursively to find a result. 
                if (splittedData[i].Any())
                    rootNode.Children.Add(((Enum)Enum.GetValues(baseData[0][rootNodeIndex].GetType()).GetValue(i), LearnData<T>(splittedData[i])));

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
