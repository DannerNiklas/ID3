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

// How to read enum values dynamically in Runtime
// Enum values as String
//string[] enumVals = data[0][0].GetType().GetEnumNames();
//foreach (var enumVal in enumVals)
//{
//    Console.WriteLine(enumVal);
//}

// Enum values as "Enum"
Array enumValues = Enum.GetValues(data[0][0].GetType());
foreach (var enumItem in enumValues)
    Console.WriteLine(enumItem);

//User gibt den Datentyp des Labels an
Type labelType = typeof(Play);
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

//Getting the label
var label = Enum.GetValues(labelType);
int[] labelValues = new int[label.Length]; //Values of the labels (How often does it appear?)

Console.WriteLine("-----------------");
for (int i = 0; i < data.Length; i++)
{
    for (int j = 0; j < data[i].Length; j++)
    {
        if (data[i][j].GetType() == labelType)
            labelValues[(int)((object)data[i][j])] += 1;
    }
}
List<int> labelValuesList = labelValues.ToList();
double totalEntropy = DecisionMethods.Entropy(labelValuesList);
Console.WriteLine("Total Entropy:" + totalEntropy);

var enumsInTable = data[0];
//Now calculating the entropy for each attribute: 

LearnDecision(labelIndex);

void LearnData(Enum[][] baseData, int rootIndex)
{
    //Get local total entropy: 
    int[] labelValues = new int[label.Length]; //Values of the labels (How often does it appear?)
    for (int i = 0; i < data.Length; i++)
    {
        for (int j = 0; j < data[i].Length; j++)
        {
            if (data[i][j].GetType() == labelType && data[i][rootIndex].GetType() == data[0][rootIndex].GetType()) //Todo: also needs to match enumElementType
                labelValues[(int)((object)data[i][j])] += 1;
        }
    }
    List<int> labelValuesList = labelValues.ToList();
    double totalEntropy = DecisionMethods.Entropy(labelValuesList);
    Console.WriteLine("Total Entropy:" + totalEntropy);

    //The information gain of the label is double MaxValue to not get problems with the index (in case the label is in the middle of the dataset) 
    List<double> informationGains = new();
    foreach (var enumInTable in enumsInTable)
    {
        List<double> entropies = new(); //contains entropy value
        List<int> appearances = new(); // the times the value appears ("Si")
        foreach (var item in Enum.GetValues(enumInTable.GetType()))
        {
            if (item.GetType() != labelType && (enumElement == null || item.GetType() != enumElement.GetType())) //preventing a loop over the tag and the current root node!
            {
                int[] currentLabelValues = new int[label.Length]; //Values of the labels (How often does it appear?)
                Console.WriteLine("-----------------");
                Console.WriteLine("Enum: " + item);
                for (int i = 0; i < data.Length; i++)
                {
                    for (int j = 0; j < data[i].Length; j++)
                    {
                        if (((int)((object)data[i][j]) == (int)((object)item) && item.GetType() == data[i][j].GetType()) && (enumElement == null || (int)((object)data[i][rootIndex]) == (int)enumElement))
                        {
                            for (int k = 0; k < data[i].Length; k++)
                            {
                                if (data[i][k].GetType() == labelType)
                                    currentLabelValues[(int)((object)data[i][k])] += 1;
                            }
                        }
                    }
                }
                //Check if the labels value is distinct
                for (int i = 0; i < currentLabelValues.Length; i++)
                {
                    if (currentLabelValues[i] == currentLabelValues.Sum() && currentLabelValues.Sum() != 0)
                    {
                        //Console.BackgroundColor = ConsoleColor.DarkRed;
                        //Console.WriteLine("Test");
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
    Console.WriteLine("Information Gains: ");
    for (int i = 0; i < informationGains.Count; i++)
    {
        Console.WriteLine(data[0][i].GetType() + ": " + informationGains[i]);
    }
    Console.WriteLine("-----------------");
    //checking, which enums value has the highest information gain:
    int rootNodeIndex = informationGains.IndexOf(informationGains.Max()); //This is the index of the item for the next knot! (This index now becomes the root of the decision tree)
    Console.WriteLine("New root node is: " + data[0][rootNodeIndex].GetType());

    //We now calculate the values for the subset: 
    //LearnDecision(rootNodeIndex);

}

void LearnDecision(int rootIndex) //ToDo: Instead of being nullable just pass down the root!
{
    if (rootIndex != null && data[0][rootIndex].GetType() != labelType)
    {
        Type rootType = data[0][rootIndex].GetType();
        foreach (var rootNodeElement in Enum.GetValues(rootType))
            CalculateTopElement(rootNodeElement, rootIndex);
    }
    else
        CalculateTopElement(null, rootIndex);
}

void CalculateTopElement(object? enumElement, int rootIndex)
{
    //Get local total entropy: 
    int[] labelValues = new int[label.Length]; //Values of the labels (How often does it appear?)
    for (int i = 0; i < data.Length; i++)
    {
        for (int j = 0; j < data[i].Length; j++)
        {
            if (data[i][j].GetType() == labelType && data[i][rootIndex].GetType() == data[0][rootIndex].GetType()) //Todo: also needs to match enumElementType
                labelValues[(int)((object)data[i][j])] += 1;
        }
    }
    List<int> labelValuesList = labelValues.ToList();
    double totalEntropy = DecisionMethods.Entropy(labelValuesList);
    Console.WriteLine("Total Entropy:" + totalEntropy);

    //The information gain of the label is double MaxValue to not get problems with the index (in case the label is in the middle of the dataset) 
    List<double> informationGains = new();
    foreach (var enumInTable in enumsInTable)
    {
        List<double> entropies = new(); //contains entropy value
        List<int> appearances = new(); // the times the value appears ("Si")
        foreach (var item in Enum.GetValues(enumInTable.GetType()))
        {
            if (item.GetType() != labelType && (enumElement == null || item.GetType() != enumElement.GetType())) //preventing a loop over the tag and the current root node!
            {
                int[] currentLabelValues = new int[label.Length]; //Values of the labels (How often does it appear?)
                Console.WriteLine("-----------------");
                Console.WriteLine("Enum: " + item);
                for (int i = 0; i < data.Length; i++)
                {
                    for (int j = 0; j < data[i].Length; j++)
                    {
                        if (((int)((object)data[i][j]) == (int)((object)item) && item.GetType() == data[i][j].GetType()) && (enumElement == null || (int)((object)data[i][rootIndex]) == (int)enumElement))
                        {
                            for (int k = 0; k < data[i].Length; k++)
                            {
                                if (data[i][k].GetType() == labelType)
                                    currentLabelValues[(int)((object)data[i][k])] += 1;
                            }
                        }
                    }
                }
                //Check if the labels value is distinct
                for (int i = 0; i < currentLabelValues.Length; i++)
                {
                    if (currentLabelValues[i] == currentLabelValues.Sum() && currentLabelValues.Sum() != 0)
                    {
                        //Console.BackgroundColor = ConsoleColor.DarkRed;
                        //Console.WriteLine("Test");
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
    Console.WriteLine("Information Gains: ");
    for (int i = 0; i < informationGains.Count; i++)
    {
        Console.WriteLine(data[0][i].GetType() + ": " + informationGains[i]);
    }
    Console.WriteLine("-----------------");
    //checking, which enums value has the highest information gain:
    int rootNodeIndex = informationGains.IndexOf(informationGains.Max()); //This is the index of the item for the next knot! (This index now becomes the root of the decision tree)
    Console.WriteLine("New root node is: " + data[0][rootNodeIndex].GetType());

    //We now calculate the values for the subset: 
    //LearnDecision(rootNodeIndex);

}

List<Enum[][]> SplitEnums(Enum[][] dataToSplit, int splitTypeIndex)
{
    Type splitType = dataToSplit[0][splitTypeIndex].GetType();

    List<Enum[][]> splittedData = new();

    foreach (var splitTypeElement in Enum.GetValues(splitType))
    {
        Enum[][] splittedDataPart = dataToSplit.Where(x => (int)((object)x[splitTypeIndex]) == (int)((object)splitTypeElement)).ToArray();
        List<Enum[]> newSplittedDataPart = new();
        for (int i = 0; i < splittedDataPart.Length; i++)
        {
            List<Enum> splittedDataPartList = splittedDataPart[i].ToList();
            splittedDataPartList.RemoveAll(x => x.Equals(splitTypeElement));
            newSplittedDataPart.Add(splittedDataPartList.ToArray());
        }

        splittedData.Add(newSplittedDataPart.ToArray());
    }
    return splittedData;
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
