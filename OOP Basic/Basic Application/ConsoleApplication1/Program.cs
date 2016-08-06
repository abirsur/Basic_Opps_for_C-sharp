using System;
using System.CodeDom;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ParentA parentA = new ParentA();
            Console.WriteLine(parentA.Message());
            ChildB childB = new ChildB();
            Console.WriteLine(childB.Message());
            ChildC childc = new ChildC();
            Console.WriteLine(childc.Message());
            ChildD childD = new ChildD();
            Console.WriteLine(childD.Message());
            Console.WriteLine("-------------------------------------------------------------");
            // ----------------------------------------------------------------------------------
            ChildAbc childAbc = new ChildAbc();
            Console.WriteLine("Example: Abstract Class");
            Console.WriteLine(childAbc.Addition(2,3));
            Console.WriteLine("-------------------------------------------------------------");
            //-----------------------------------------------------------------------------------
            Console.WriteLine("Example: Interface and Classes");
            IContract1 contract1 = new ChildBcd(); 
            Console.WriteLine(contract1.Addition(2,3));
            Console.WriteLine("-------------------------------------------------------------");
            //-----------------------------------------------------------------------------------
            contract1 = new ChildDef();
            Console.WriteLine(contract1.Addition(2, 3));
            Console.WriteLine("-------------------------------------------------------------");

            ExamplePartialClass examplePartialClass = new ExamplePartialClass();
            Console.WriteLine(examplePartialClass.Service());
            Console.WriteLine(examplePartialClass.Shop());
            Console.WriteLine("-------------------------------------------------------------");
            //-----------------------------------------------------------------------------------
            Console.WriteLine("----------------------Operator Overloading-------------------");
            MyCalculation myCalculation1 = new MyCalculation(7);
            MyCalculation myCalculation2 = new MyCalculation(2);
            var t = myCalculation1 + myCalculation2;
            Console.WriteLine(t);
            Console.ReadLine();
        }
    }

    public class ParentA
    {
        public ParentA()
        {
        }

        public virtual string Message()
        {
            return "Hi now I am in ParentA class";
        }
    }

    public class ChildB:ParentA
    {
        public ChildB()
        {
        }

        public override string Message()
        {
            return "Hi now I am in ChildB class";
        }
    }

    public class ChildC : ParentA
    {
        public ChildC()
        {
        }

        public override string Message()
        {
            return "Hi now I am in ChildC class";
        }
    }

    public class ChildD : ParentA
    {
        public ChildD()
        {
        }

        public new string Message()
        {
            return "Hi now I am in ChildD class";
        }
    }

    public abstract class ClassAbstract
    {
        public int Id { get; set; }
        public virtual int Addition(int a, int b)
        {
            return a + b;
        }
    }

    public class ChildAbc : ClassAbstract
    {
        public new long Id { get; set; }

        public ChildAbc()
        {
        }
       
    }

    public interface IContract1
    {
        int Addition(int number1, int number2);
    }

    public class ChildBcd:IContract1
    {
        public int Addition(int a, int b)
        {
            return a + b;
        }
    }

    public class ChildDef : IContract1
    {
        public int Addition(int a, int b)
        {
            return a*b;
        }
    }

    public partial class ExamplePartialClass
    {
        public string Shop()
        {
            return "hi I am delcared in parial 1, my method is: Shop";
        }
    }
    public partial class ExamplePartialClass
    {
        public string Service()
        {
            return "hi I am delcared in parial 2, my method is: Service";
        }
    }

    public class MyCalculation
    {
        private int Num1 { get; set; }
        
        public MyCalculation(int num1)
        {
            this.Num1 = num1;
        }

        public static int operator +(MyCalculation number1, MyCalculation number2)
        {
            return (number1.Num1 - number2.Num1);
        }
    }

}
