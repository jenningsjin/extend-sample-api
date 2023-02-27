using System.ComponentModel;

namespace Util {
    static class util {

        // Generic Function that Prints all the Properties of a Given Object.
        public static void PrintProperties(object obj) {
            // Helper Function to 
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(obj);
                Console.WriteLine("{0}={1}", name, value);
            }
        }
    }
}