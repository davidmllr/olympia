using System.Linq;

public class ArrayUtils
{
    /// <summary>
    ///     Concatenates two or more arrays into a single one.
    ///     Taken from here: https://www.ryadel.com/en/asp-net-merge-concatenate-array-two-arrays-c-sharp-linq/
    /// </summary>
    public static T[] Concat<T>(params T[][] arrays)
    {
        var result = new T[arrays.Sum(a => a.Length)];
        var offset = 0;
        for (var x = 0; x < arrays.Length; x++)
        {
            arrays[x].CopyTo(result, offset);
            offset += arrays[x].Length;
        }

        return result;
    }
}