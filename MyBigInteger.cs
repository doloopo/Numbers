using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _BigInteger
{
    public class UnfinishedException : Exception {}

    public class MyBigInteger
    {
        public enum StringType {Hex, Dec}

        // Does List<T> cause performance loss? 
        public bool isNegative { get; set; }

        // public int rawDataCount { get { return rawData.Count; } }
        // Type uint used for easy debugging. 
        public List<uint> rawData = new List<uint>();
        protected List<uint> zeroData = new List<uint> { 0 };

        // clearEmpty() is used to clear empty int listitems,
        // from the last (also the 最高位) to the first listitem.
        protected void clearEmpty()
        {
            if (rawData.Count > 1)
            {
                for (int i = rawData.Count - 1; i >= 0; i--)
                {
                    if (rawData[i] == 0) rawData.RemoveAt(i);
                    else return;
                }
            }
        }

        // For debug purposes. 
        public void PrintRaw()
        {
            foreach (uint i in rawData) Console.Write(i + ". ");
            Console.WriteLine();
        }
        public void ComparerTest()
        {
            const int TEST_NUM = 100000;
            Random rd = new Random();
            for (int i = 0; i < TEST_NUM; i++)
            {
                long a = rd.NextInt64(), b = rd.NextInt64();
                if ((a > b) == (new MyBigInteger(a) > new MyBigInteger(b)) &&
                    (a < b) == (new MyBigInteger(a) < new MyBigInteger(b)) &&
                    (a == b) == (new MyBigInteger(a) == new MyBigInteger(b))) { }
                else Console.WriteLine("On attempt #{0}: You fucked. a = {1}, b = {2}. ",
                i, a, b);
            }
            Console.WriteLine("OK. ");
        }

        // In normal cases, the rawData should be at least {0}, 
        // with the isNegative = false. 
        public MyBigInteger()
        {
            rawData = zeroData;
            isNegative = false;
        }
        public MyBigInteger(int length, uint fill)
        {
            isNegative = false;
            for (int i = 0; i < length; i++) rawData.Add(fill);
        }
        public MyBigInteger(int length, MyBigInteger fill)
        {
            if (length < fill.rawData.Count)
                throw new OverflowException("Fill is longer than specified length. ");
            isNegative = fill.isNegative;
            rawData = fill.rawData;

            while (this.rawData.Count < fill.rawData.Count)
            {
                this.rawData.Add(0);
            }
        }
        public MyBigInteger(long value)
        {
            isNegative = (value < 0);
            value = Math.Abs(value);
            rawData.Add((uint)(value % 0x100000000));
            rawData.Add((uint)(value >> 32));
            clearEmpty();
        }
        // Only accept numbers like "123456789", "-123456789". 
        // Don't use: "123,456,789", "123456.789" etc. 
        // Need to be improved. 
        public MyBigInteger(string value, StringType type)
        {
            switch (type) {
                case StringType.Dec:
                    throw new UnfinishedException();
                case StringType.Hex:
                    throw new UnfinishedException();
            }
        }
        public MyBigInteger(MyBigInteger t)
        {
            isNegative = t.isNegative;
            rawData = t.rawData;
        }

        // Operators
        public MyBigInteger GetAbs()
        {
            MyBigInteger res = new MyBigInteger(this);
            res.isNegative = false;
            return res;
        }
        public static MyBigInteger AbsAdd(MyBigInteger a, MyBigInteger b)
        {
            // Initialise
            MyBigInteger res;
            if (a.rawData.Count >= b.rawData.Count) res = new MyBigInteger(a.rawData.Count + 1, 0);
            else res = new MyBigInteger(b.rawData.Count + 1, 0);

            MyBigInteger ta = new MyBigInteger(res.rawData.Count, a),
            tb = new MyBigInteger(res.rawData.Count, b);

            for (int i = 0; i < res.rawData.Count; i++)
            {
                bool carry = false;
                // Current = res + a + b
                res.rawData[i] = (uint)(((long)res.rawData[i] + (long)ta.rawData[i] + (long)tb.rawData[i]) % 0x100000000);
                // Carry. At the last position there'll be no carry so
                // the overflow would be avoided.  
                carry = ((long)res.rawData[i] + (long)ta.rawData[i] + (long)tb.rawData[i]) >= 0x100000000;
                if (carry)
                    res.rawData[i + 1]++;
            }
            res.clearEmpty();
            return res;
        }
        public static MyBigInteger AbsSubtract(MyBigInteger great, MyBigInteger less) {
            if (great < less) throw new InvalidDataException("The great is less than the less. ");
            MyBigInteger res = new MyBigInteger(great); 
            throw new UnfinishedException(); 
            return res; 
        }

        public static bool operator >(MyBigInteger a, MyBigInteger b)
        {
            if (a.isNegative != b.isNegative) return !a.isNegative;
            else
            {
                if (a.rawData.Count != b.rawData.Count)
                    // a.c > b.c    a.neg    result
                    // true         true     false
                    // true         false    true
                    // false        true     true
                    // false        false    false
                    return (a.rawData.Count > b.rawData.Count) ^ a.isNegative;
                else
                {
                    for (int i = a.rawData.Count - 1; i >= 0; i--)
                    {
                        if (a.rawData[i] > b.rawData[i]) return true ^ a.isNegative;
                        if (a.rawData[i] < b.rawData[i]) return false ^ a.isNegative;
                    }
                    return false;
                }
            }
        }
        public static bool operator <(MyBigInteger a, MyBigInteger b)
        {
            if (a.isNegative != b.isNegative) return a.isNegative;
            else
            {
                if (a.rawData.Count != b.rawData.Count)
                    return (a.rawData.Count < b.rawData.Count) ^ a.isNegative;
                else
                {
                    for (int i = a.rawData.Count - 1; i >= 0; i--)
                    {
                        if (a.rawData[i] > b.rawData[i]) return false ^ a.isNegative;
                        if (a.rawData[i] < b.rawData[i]) return true ^ a.isNegative;
                    }
                    return false;
                }
            }
        }
        public static bool operator >=(MyBigInteger a, MyBigInteger b)
        {
            return (a > b) || (a == b);
        }
        public static bool operator <=(MyBigInteger a, MyBigInteger b)
        {
            return (a < b) || (a == b);
        }
        public static MyBigInteger operator +(MyBigInteger a, MyBigInteger b)
        {
            throw new UnfinishedException();
        }
        public static bool operator ==(MyBigInteger a, MyBigInteger b)
        {
            if (a.isNegative == b.isNegative)
                if (a.rawData.Count == b.rawData.Count)
                    for (int i = 0; i < a.rawData.Count; i++)
                        if (a.rawData[i] == b.rawData[i]) return true;
            return false;
        }
        public static bool operator !=(MyBigInteger a, MyBigInteger b)
        {
            if (a == b) return false;
            else return true;
        }
        public override bool Equals(object? obj)
        {
            return obj == null ? false : (MyBigInteger)obj == this;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // Types
        public static explicit operator long(MyBigInteger value)
        {
            int c = value.rawData.Count;
            if (c > 2)
            {
                throw new OverflowException("Type 'long' cannot contain the value. ");
            }
            else if (c == 2)
            {
                return (((long)value.rawData[1]) << 32) + (long)value.rawData[0];
            }
            else if (c == 1)
            {
                return value.rawData[0];
            }
            else
            {
                throw new InvalidDataException("The data is empty. ");
            }
        }
        public static implicit operator MyBigInteger(long value)
        {
            return new MyBigInteger(value);
        }
        public string ToHexString() {
            string res = ""; 
            for (int i = 0; i < rawData.Count; i++) {
                if (i != rawData.Count - 1) res = rawData[i].ToString("x8") + res;
                else res = rawData[i].ToString("x") + res;  
            }
            return res; 
        }
        // TODO: public string ToDecString()
    }
}