using System;

namespace Taurit.Toolkit.ProcessTodoistInbox.RemoveArchivedItems
{
    /// <summary>
    ///     Requests deletion of all completed or archived items to reduce privacy risk in case of data leak (eg. from
    ///     clients).
    /// </summary>
    internal class Program
    {
        private static void Main(String[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Expected arguments:");
                Console.WriteLine("[0] Todoist API key");
                Console.WriteLine("Nothing was done. Exiting.");
                return;
            }

            String apiKey = args[0];
            var remover = new CompletedItemsRemover(apiKey);
            remover.RemoveAllCompletedItems();
        }

    }
}