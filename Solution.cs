/// Author: Pooja Shah
/// Date of Creation: Feb 13, 2017
/// This program is uses Factory Design pattern to create a Chained Hash Table

using System;
using System.Collections;
using System.Collections.Generic;

namespace RIT_CS
{
    /// <summary>
    /// An exception used to indicate a problem with how
    /// a HashTable instance is being accessed
    /// </summary>
    public class NonExistentKey<Key> : Exception
    {
        /// <summary>
        /// The key that caused this exception to be raised
        /// </summary>
        public Key BadKey { get; private set; }

        /// <summary>
        /// Create a new instance and save the key that
        /// caused the problem.
        /// </summary>
        /// <param name="k">
        /// The key that was not found in the hash table
        /// </param>
        public NonExistentKey(Key k) :
            base("Non existent key in HashTable: " + k)
        {
            BadKey = k;
        }

    }

    /// <summary>
    /// An associative (key-value) data structure.
    /// A given key may not appear more than once in the table,
    /// but multiple keys may have the same value associated with them.
    /// Tables are assumed to be of limited size are expected to automatically
    /// expand if too many entries are put in them.
    /// </summary>
    /// <param name="Key">the types of the table's keys (uses Equals())</param>
    /// <param name="Value">the types of the table's values</param>
    interface Table<Key, Value> : IEnumerable<Key>
    {
        /// <summary>
        /// Add a new entry in the hash table. If an entry with the
        /// given key already exists, it is replaced without error.
        /// put() always succeeds.
        /// (Details left to implementing classes.)
        /// </summary>
        /// <param name="k">the key for the new or existing entry</param>
        /// <param name="v">the (new) value for the key</param>
        void Put(Key k, Value v);

        /// <summary>
        /// Does an entry with the given key exist?
        /// </summary>
        /// <param name="k">the key being sought</param>
        /// <returns>true iff the key exists in the table</returns>
        bool Contains(Key k);

        /// <summary>
        /// Fetch the value associated with the given key.
        /// </summary>
        /// <param name="k">The key to be looked up in the table</param>
        /// <returns>the value associated with the given key</returns>
        /// <exception cref="NonExistentKey">if Contains(key) is false</exception>
        Value Get(Key k);
    }

    class TableFactory
    {
        /// <summary>
        /// Create a Table.
        /// (The student is to put a line of code in this method corresponding to
        /// the name of the Table implementor s/he has designed.)
        /// </summary>
        /// <param name="K">the key type</param>
        /// <param name="V">the value type</param>
        /// <param name="capacity">The initial maximum size of the table</param>
        /// <param name="loadThreshold">
        /// The fraction of the table's capacity that when
        /// reached will cause a rebuild of the table to a 50% larger size
        /// </param>
        /// <returns>A new instance of Table</returns>
        public static Table<K, V> Make<K, V>(int capacity = 100, double loadThreshold = 0.75)
        {
            return new LinkedHashTable<K, V>(capacity, loadThreshold);
        }
    }

    /// <summary>
    /// This is a Node class which will hold key-value pairs in the hash table.
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    public class Node<Key, Value>
    {
        /// <summary>
        /// Stores the key in the node.
        /// </summary>
        public Key key { get; set; }
        /// <summary>
        /// Stores the value in the node.
        /// </summary>
        public Value value { get; set; }
        /// <summary>
        /// A constructor to create a new Node.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Node(Key key, Value value)
        {
            this.key = key;
            this.value = value;
        }
    }

    /// <summary>
    /// This class creates a chained hash table.
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    public class LinkedHashTable<Key, Value> : Table<Key, Value>
    {
        /// <summary>
        /// Stores the size of the array in the hash table.
        /// </summary>
        public int capacity { get; set; }

        /// <summary>
        /// Stores the number of entries in the hash table.
        /// </summary>
        public int size { get; set; }

        /// <summary>
        /// Stores the load threshold value and rehashing is performed
        /// if the load factor exceeds the load threshold.
        /// </summary>
        public double loadThreshold { get; set; }

        /// <summary>
        /// Declare an array for hash table
        /// </summary>
        List<List<Node<Key, Value>>> bucketList;

        /// <summary>
        /// A constructor to create a chained hash table
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="loadThreshold"></param>
        public LinkedHashTable(int capacity, double loadThreshold)
        {
            /// Initializes the hash table
            this.bucketList = new List<List<Node<Key, Value>>>(capacity);

            /// Sets the size of array to given capacity
            this.capacity = capacity;

            /// Sets the load threshold
            this.loadThreshold = loadThreshold;

            /// Initializes empty hash table by setting its size to 0.
            size = 0;
            
            /// Initializes the array positions to null
            for(int index = 0; index<capacity; index++)
            {
                bucketList.Add(null);
            }
        }

        /// <summary>
        /// Calculates the load factor of the hash table
        /// to determine whether the rehashing should be performed or not.
        /// </summary>
        /// <returns>Returns the calculated load factor</returns>
        public double GetLoadFactor()
        {
            /// Calculates the load factor by 
            /// dividing the number of entries in the hash table 
            /// by the size of the array.
            double loadFactor = (double)size / capacity;
            return loadFactor;
        }

        /// <summary>
        /// Gets the position where the key-value pair should be
        /// inserted in the hash table by calculating hash code
        /// on the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>
        /// Returns the absolute position where element will be inserted
        /// </returns>
        public int GetBucketPosition(Key key)
        {
            /// Gets the hashcode of the given key.
            int hashCode = key.GetHashCode();

            /// Finds the position of the key in the hash table.
            int position = hashCode % capacity;
            return Math.Abs(position);
        }

        /// <summary>
        /// Checks if the given key already exists in the hash table.
        /// If the given key already exists, it returns true
        /// else false.
        /// </summary>
        /// <param name="k"></param>
        /// <returns> 
        /// True or false to determine if key is present in the hash table.
        /// </returns>
        public bool Contains(Key k)
        {
            /// Finds the position where the given key 
            /// may exists in the hash table.
            int position = GetBucketPosition(k);

            /// Retrieves the list in that particular position.
            List<Node<Key, Value>> list = bucketList[position];

            /// Checks if the list is empty. If yes, it returns false
            /// as the given key is not present in the hash table.
            if(list == null)
            {
                return false;
            }

            /// If list is not null, it iterates through the list
            /// to search if the given key exists in the hash table.
            for(int index =0; index < list.Count; index++)
            {
                Node<Key, Value> currentNode = list[index];
                if(currentNode.key.Equals(k))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Inserts the given key-value pair at a specified postion in the hash table.
        /// If given key is already present in the hash table, then replace it with the new value.
        /// If given key is null, ArgumentNullException is thrown.
        /// </summary>
        /// <param name="k"></param>
        /// <param name="v"></param>
        public void Put(Key k, Value v)
        {
            /// If given key is null, it throws ArgumentNullException.
            if(k == null)
            {
                throw new ArgumentNullException();
            }

            /// Checks if the given key is present in the hash table.
            bool isPresent = Contains(k);

            /// If the given key is present, the value is replaced with the given value.
            if(isPresent == true)
            {
                int pos = GetBucketPosition(k);
                List<Node<Key, Value>> elements = bucketList[pos];

                /// Iterates through the list to find the key 
                /// where the value can be replaced.
                for (int currentIndex = 0; currentIndex < elements.Count; currentIndex++)
                {
                    foreach(var node in elements)
                    {
                        if (node.key.Equals(k))
                        {
                            node.value = v;
                            break;
                        }
                    }
                }
                bucketList[pos] = elements;
                Console.WriteLine("Key " + k + " already exists. Value is replaced as: " + v);
                return;
            }

            /// Calculates the load factor of the hash table
            double loadFactor = GetLoadFactor();

            /// If the load factor exceeds the load threshold,
            /// rehashing operation is performed.
            if(loadFactor > loadThreshold)
            {
                Rehash();
            }

            /// Gets the position of hash table
            /// where the key will be inserted.
            int position = GetBucketPosition(k);

            /// Inserts the key-value pair in the hash table at a particular position.
            /// Increments the size of the array after insert.
            if(bucketList[position] == null)
            {
                bucketList[position] = new List<Node<Key, Value>>();
                bucketList[position].Add(new Node<Key, Value>(k, v));
                size++;
            }
            else
            {
                bucketList[position].Add(new Node<Key, Value>(k, v));
                size++;
            }
            Console.WriteLine("Key " + k + " - > Value " + v + " is inserted.");
        }

        /// <summary>
        /// Performs rehashing of hash table everytime expansion of the array
        /// is required. The array is expanded 50% of the original array.
        /// </summary>
        public void Rehash()
        {
            Console.WriteLine("Rehashing....");
            /// Calculates the new size of the array
            int new_capacity = (int)(1.5 * capacity);
            List<List<Node<Key, Value>>> temp = bucketList;
            bucketList = new List<List<Node<Key, Value>>>(new_capacity);

            /// Sets the size of the array to new calculated size of array.
            capacity = new_capacity;
            Console.WriteLine("After rehashing, the size of array is: " + capacity);
            for(int i=0; i<capacity; i++)
            {
                bucketList.Add(null);
            }
            List<Node<Key, Value>> elements = new List<Node<Key, Value>>();

            /// Retrieves all the elements from the original hash table
            for (int index = 0; index < temp.Count; index++)
            {
                if(temp[index] == null)
                {
                    continue;
                }
                else
                {
                    elements.AddRange(temp[index]);
                }
            }

            /// Inserts the elements in the rehashed hash table.
            foreach(var node in elements)
            {
                Put(node.key, node.value);
            }
        }

        /// <summary>
        /// Gets the value associated with the given key.
        /// If key is not present in the hash table, NonExistentKey
        /// is thrown.
        /// </summary>
        /// <param name="k"></param>
        /// <returns>Returns the corresponding value of the given key</returns>
        public Value Get(Key k)
        {
            /// Checks if the given key exists in the hash table.
            /// If it doesn't exist, NonExistentKey is thrown.
            if (!Contains(k))
            {
                throw new NonExistentKey<Key>(k);
            }
            else
            {
                /// Gets the position of the array where the given key exists.
                int position = GetBucketPosition(k);
                List<Node<Key, Value>> list = bucketList[position];
                for (int index = 0; index < list.Count; index++)
                {
                    Node<Key, Value> currentNode = list[index];

                    /// Once the key is found, returns the value
                    if (currentNode.key.Equals(k))
                    {
                        return currentNode.value;
                    }
                }
            }
            return default(Value);
        }

        /// <summary>
        /// Enumerates through the keys of the hash table.
        /// </summary>
        /// <returns>
        /// Returns the list of keys in hash table.
        /// </returns>
        public IEnumerator<Key> GetEnumerator()
        {
            /// Iterates through the array of the hash table and
            /// appends the key-value pair in the list.
            for (var i = 0; i < capacity; i++)
            {
                if(bucketList[i] != null)
                {
                    foreach(var elem in bucketList[i])
                    {
                        yield return elem.key;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the enumerator of the hashtable.
        /// </summary>
        /// <returns>
        /// Returns the enumerator of the hashtable.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return bucketList.GetEnumerator();
        }
    }

    /// <summary>
    /// This is a test class which includes the test data 
    /// to check the functionality of hash table.
    /// </summary>
    class TestTable
    {
        public static void test()
        {
            /// Check for different generic hash table
            Table<int, String> h = TableFactory.Make<int, String>(10, 0.75);

            Console.WriteLine("=================INSERTING KEY-VALUE PAIRS=================");
            /// Add elements in the hash table
            h.Put(4000, "Welcome4000");
            h.Put(5000, "Welcome5000");

            ///  Add 3000 elements in hash table and check for rehashing
            for (int i = 0; i < 3000; i++)
            {
                h.Put(i, "Hello" + i);
            }

            Console.WriteLine("=================INSERTING EXISTING KEY=================");
            /// Puts the existing key in the hash table. Checks if the value is replaced for existing key
            h.Put(2999, "ValueChanged2999");

            try
            {
                /// Prints elements in the hash table
                Console.WriteLine("=================PRINTING KEY-VALUE PAIRS=================");
                foreach (int key in h)
                {
                    Console.WriteLine(key + " -> " + h.Get(key));
                }

                /// Checks if the key is present in the hash table
                Console.WriteLine("=================CHECKS IF KEY EXISTS=================");
                bool isKeyPresent = h.Contains(3400);
                Console.WriteLine("Is 3400 present in the hash table? : " + isKeyPresent);

                /// Checks for non-existent key in hash table
                Console.WriteLine("=================CHECKS FOR NON-EXISTENT KEY=================");
                Console.Write("3500 -> ");
                Console.WriteLine(h.Get(3500));
            }
            catch (NonExistentKey<int> nek)
            {
                Console.WriteLine(nek.Message);
                Console.WriteLine(nek.StackTrace);
            }

            Table<String, String> hashTable = TableFactory.Make<String, String>(4, 0.5);
            /// Tries to insert null key in hash table
            Console.WriteLine("=================INSERTING NULL KEY=================");
            try
            {
                hashTable.Put(null, "NullKey");
            }
            catch (ArgumentNullException argNullEx)
            {
                Console.WriteLine("Key cannot be null");
                Console.WriteLine(argNullEx.StackTrace);
            }
        }
    }

    /// <summary>
    /// Main class
    /// </summary>
    class MainClass
    {
        public static void Main(string[] args)
        {
            Table<String, String> ht = TableFactory.Make<String, String>(4, 0.5);
            ht.Put("Joe", "Doe");
            ht.Put("Jane", "Brain");
            ht.Put("Chris", "Swiss");

            try
            {
                foreach (String first in ht)
                {
                    Console.WriteLine(first + " -> " + ht.Get(first));
                }
                Console.WriteLine("=========================");

                ht.Put("Wavy", "Gravy");
                ht.Put("Chris", "Bliss");
                foreach (String first in ht)
                {
                    Console.WriteLine(first + " -> " + ht.Get(first));
                }
                Console.WriteLine("=========================");

                Console.Write("Jane -> ");
                Console.WriteLine(ht.Get("Jane"));
                Console.Write("John -> ");
                Console.WriteLine(ht.Get("John"));
            }
            catch (NonExistentKey<String> nek)
            {
                Console.WriteLine(nek.Message);
                Console.WriteLine(nek.StackTrace);
            }

            /// Calls the test function
            TestTable.test();
            Console.ReadLine();
        }
    }
}