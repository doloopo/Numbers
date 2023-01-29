using System.Numerics;
using _BigInteger;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// mbi.PrintRaw();
// mbi.ComparerTest(); 
// Console.WriteLine(mbi.ToHexString()); 
TestA();

static void TestA()
{
    BigInteger a = new BigInteger(11451455555555);
    BigInteger a2 = new BigInteger(5646454556456);
    a = a - a2; 
    Console.WriteLine(a.ToString("x"));
    MyBigInteger b = new MyBigInteger(11451455555555);
    MyBigInteger b2 = new MyBigInteger(5646454556456);
    b = b - b2;
    Console.WriteLine(b.ToHexString());
}

static void TestB() {
    MyBigInteger a = new MyBigInteger(456); 
    Console.WriteLine((- a).ToHexString()); 
    Console.WriteLine(a.ToHexString());
}