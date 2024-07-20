public delegate void MyDelegate(string message);

MyDelegate del = MyFunction;

public static void MyFunction(string message){
    Console.WriteLine(message);
}