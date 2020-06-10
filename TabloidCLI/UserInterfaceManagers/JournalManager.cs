using System;
using System.Collections.Generic;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    public class JournalManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;
        private JournalRepository _journalRepository;
        private string _connectionString;

        public JournalManager(IUserInterfaceManager parentUI, string connectionString)
        {
            _parentUI = parentUI;
            _journalRepository = new JournalRepository(connectionString);
            _connectionString = connectionString;
        }

        public IUserInterfaceManager Execute()
        {
            Console.WriteLine("Journal Menu");
            Console.WriteLine(" 1) List Journal Entries");
            Console.WriteLine(" 2) Journal Entry Details");
            Console.WriteLine(" 3) Add Journal Entry");
            Console.WriteLine(" 4) Edit Journal Entry");
            Console.WriteLine(" 5) Remove Journal Entry");
            Console.WriteLine(" 0) Go Back");

            Console.Write("> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    List();
                    return this;
                case "2":
                    Journal entry = Choose();
                    if (entry == null)
                    {
                        return this;
                    }
                    else
                    {
                        return new JournalDetailManager(this, _connectionString, entry.Id);
                    }
                case "3":
                    Add();
                    return this;
                case "4":
                    Edit();
                    return this;
                case "5":
                    Remove();
                    return this;
                case "0":
                    return _parentUI;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this;
            }
        }

        private void List()
            //creates a list of journal obj called journals. Then iterates through list, writing each one to the console.
        {
            List<Journal> journals = _journalRepository.GetAll();
            foreach (Journal journal in journals)
            {
                Console.WriteLine(journal);
            }
        }

        private Journal Choose(string prompt = null)
            //Displays a list of journal entry titles and prompts user to choose a journal entry. When entry is chosen the details for that journal entry are diplayed.
        {
            if (prompt == null)
            {
                prompt = "Please choose a Journal Entry:";
            }

            Console.WriteLine(prompt);

            List<Journal> journals = _journalRepository.GetAll();

            for (int i = 0; i < journals.Count; i++)
            {
                Journal journal = journals[i];
                Console.WriteLine($" {i + 1}) {journal.Title}");
            }
            Console.Write("> ");

            string input = Console.ReadLine();
            try
            {
                int choice = int.Parse(input);
                return journals[choice - 1];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Selection");
                return null;
            }
        }

        //Adding a new journal entry. User is prompted to enter in data that will be stored in variables. 
        //Current time will be saved with data. Then it will be inserted into the database. 
        private void Add()
        {
            Console.WriteLine("New Journal Entry");
            Journal journal = new Journal();

            Console.WriteLine("Title: ");
            journal.Title = Console.ReadLine();

            Console.WriteLine("Content: ");
            journal.Content = Console.ReadLine();

            Console.WriteLine("Your journal entry time will be saved at the current time.");
            journal.CreateDateTime = DateTime.Now;

            _journalRepository.Insert(journal);
        }
        

        //Editing a journal entry. 
        private void Edit()
        {
            Journal journalToEdit = Choose("Which journal entry would you like to edit?");
            if (journalToEdit == null)
            {
                return;
            }

            //if user doesn't leave blank or unchanged then the data the user entered will be stored in title and passed into database 
            Console.WriteLine();
            Console.WriteLine("New Title (blank to leave unchanged: ");
            string title = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(title))
            {
                journalToEdit.Title = title;
            }
            Console.WriteLine("Update Content (blank to leave unchanged: ");
            string content = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(content))
            {
                journalToEdit.Content = content;
            }
            Console.WriteLine("Your journal entry time will be updated to the current time.");
            

            journalToEdit.CreateDateTime = DateTime.Now;
            

            _journalRepository.Update(journalToEdit);
        }

        private void Remove()
        {
            Journal journalToDelete = Choose("Which journal would you like to remove?");
            if (journalToDelete != null)
            {
                _journalRepository.Delete(journalToDelete.Id);
            }
        }
    }
}
