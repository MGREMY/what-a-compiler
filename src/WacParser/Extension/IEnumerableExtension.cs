namespace WacParser.Extension;

public static class IEnumerableExtension
{
    extension<T>(IEnumerable<T> enumerable)
    {
        public IEnumerable<IEnumerable<T>> Split(Func<T, bool> predicate)
        {
            var currentGroup = new List<T>();
            foreach (var item in enumerable)
            {
                if (predicate(item) && currentGroup.Any())
                {
                    yield return currentGroup;
                    currentGroup.Clear();
                }

                currentGroup.Add(item);
            }

            if (currentGroup.Any())
            {
                yield return currentGroup;
            }
        }
    }
}