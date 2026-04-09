// See https://aka.ms/new-console-template for more information

using CSharpFundamentals;
using System.Text.Json;

namespace ToDoApp
{
    class Program
    {
        static List<Note> allNotes = new List<Note>();
        static string fileName = "notes.txt";
        static void Main(string[] args)
        {
            ReadAndSaveNotes();

            while (true)
            {
                Console.WriteLine("=== Notes Application ===");
                Console.WriteLine("1. Add a new note");
                Console.WriteLine("2. View notes");
                Console.WriteLine("3. Mark note as done");
                Console.WriteLine("4. Delete a note by Id");
                Console.WriteLine("5. Clear all notes");
                Console.WriteLine("6. Search by content");
                Console.WriteLine("8. Sorting");
                Console.WriteLine("10. Exit");
                Console.WriteLine("Select your option: ");
                int enteredOption = int.Parse(Console.ReadLine());
                switch (enteredOption)
                {
                    case 1:
                        AddNewNote();
                        break;
                    case 2:
                        ViewNotes();
                        break;
                    case 3:
                        MarkNoteDone();
                        break;
                    case 4:
                        DeleteOneNote();
                        break;
                    case 5:
                        DeleteAllNotes();
                        break;
                    case 6:
                        SearchByKeyword();
                        break;
                    case 8:
                        SortNotes();
                        break;
                    case 10:
                        return;
                    default:
                        Console.WriteLine("You enter the option isn't in the list. Please try again!");
                        break;
                }
            }
        }

        public static void ReadAndSaveNotes()
        {
            if (!File.Exists("notes.txt"))
            {
                File.Create("notes.txt");
            }
            var text = File.ReadAllText("notes.txt");
            if (!string.IsNullOrWhiteSpace(text))
            {
                allNotes = JsonSerializer.Deserialize<List<Note>>(text);
            }
            //foreach (var note in notes)
            //{
            //    string[] noteSplitted = note.Split(",");
            //    Note saveNote = new Note
            //    {
            //        Id = int.Parse(noteSplitted[0].Trim()),
            //        CreatedAt = DateTime.Parse(noteSplitted[1].Trim()),
            //        Content = noteSplitted[2].Trim(),
            //        Status = Enum.Parse<NoteStatus>(noteSplitted[3].Trim())
            //    };

            //    allNotes.Add(saveNote);
            //}

        }

        public static void AddNewNote()
        {
            string newContent;
            Console.WriteLine("Enter your note content: ");

            while (true)
            {
                newContent = Console.ReadLine();

                if (string.IsNullOrEmpty(newContent))
                {
                    Console.WriteLine("Your content cannot empty, please try again!");
                }
                else
                {
                    break;
                }
            }

            int newestId = allNotes.Count == 0 ? 0 : allNotes.Max(n => n.Id);

            Note newNote = new Note
            {
                Id = newestId + 1,
                CreatedAt = DateTime.Now,
                Content = newContent,
                Status = NoteStatus.NOT_DONE
            };

            allNotes.Add(newNote);

            WriteToFile();

            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine($"{newNote.Id}, {newNote.CreatedAt}, {newNote.Content}, {newNote.Status}");

            //File.AppendAllText("notes.txt", sb.ToString());

        }

        public static void ViewNotes()
        {
            try
            {
                string enteredString;
                Console.WriteLine("1. View all notes");
                Console.WriteLine("2. View only NOT DONE notes");
                Console.WriteLine("Choose an option: ");
                while (true)
                {
                    enteredString = Console.ReadLine();
                    if (string.IsNullOrEmpty(enteredString))
                    {
                        Console.WriteLine("Your option cannot empty, please try again!");
                    }
                    else
                    {
                        break;
                    }
                }

                int enteredOption = int.Parse(enteredString);

                switch (enteredOption)
                {
                    case 1:
                        foreach (var item in allNotes)
                        {
                            Console.WriteLine(item.ToString());
                        }
                        break;
                    case 2:
                        var filteredNotes = allNotes.Where(n => n.Status == NoteStatus.NOT_DONE);
                        if (!filteredNotes.Any())
                        {
                            Console.WriteLine("No notes found");
                        }
                        else
                        {
                            foreach (var item in filteredNotes)
                            {
                                Console.WriteLine(item.ToString());
                            }
                        }

                        break;
                    default:
                        Console.WriteLine("You enter the option isn't in the list. Please try again!");
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void MarkNoteDone()
        {
            Console.WriteLine("Enter NoteId you want to mark as done:");
            try
            {
                string enteredString;
                while (true)
                {
                    enteredString = Console.ReadLine();
                    if (string.IsNullOrEmpty(enteredString))
                    {
                        Console.WriteLine("Note Id cannot empty, please try again!");
                    }
                    else
                    {
                        break;
                    }
                }

                if (!int.TryParse(enteredString, out int enteredNoteId))
                {
                    Console.WriteLine("Id must be a numeric. Please try again!");
                }

                Note? note = allNotes.Find(n => n.Id == enteredNoteId);

                if (note == null)
                {
                    Console.WriteLine("Cannot find your note id");
                    return;
                }

                if (note.Status == NoteStatus.DONE)
                {
                    Console.WriteLine("Note already done!");
                    return;
                }

                Console.Write("Are you sure you want to mark this note as DONE? (Y/N): ");
                string confirmAnswer = Console.ReadLine();

                if (confirmAnswer == "Y" || confirmAnswer == "y")
                {
                    note.Status = NoteStatus.DONE;

                    WriteToFile();

                    //StringBuilder sb = new StringBuilder();

                    //foreach (var noteItem in allNotes)
                    //{
                    //    sb.AppendLine($"{noteItem.Id}, {noteItem.CreatedAt}, {noteItem.Content}, {noteItem.Status}");
                    //}

                    //File.WriteAllText("notes.txt", sb.ToString());

                    Console.WriteLine("Note marked as DONE.");
                }


                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void DeleteOneNote()
        {
            Console.WriteLine("Enter NoteId you want to delete:");
            try
            {
                string enteredString;
                while (true)
                {
                    enteredString = Console.ReadLine();
                    if (string.IsNullOrEmpty(enteredString))
                    {
                        Console.WriteLine("Note Id cannot empty, please try again!");
                    }
                    else
                    {
                        break;
                    }
                }

                if (!int.TryParse(enteredString, out int enteredNoteId))
                {
                    Console.WriteLine("Id must be a numeric. Please try again!");
                }

                Note? note = allNotes.Find(n => n.Id == enteredNoteId);

                if (note == null)
                {
                    Console.WriteLine("Cannot find your note id");
                    return;
                }

                Console.Write("Are you sure you want to DELETE this note? (Y/N): ");
                string confirmAnswer = Console.ReadLine();

                if (confirmAnswer == "Y" || confirmAnswer == "y")
                {
                    allNotes.Remove(note);

                    for (int i = 0; i < allNotes.Count; i++)
                    {
                        allNotes[i].Id = i + 1;
                    }

                    WriteToFile();

                    //StringBuilder sb = new StringBuilder();

                    //for (int i = 0; i < allNotes.Count; i++)
                    //{
                    //    allNotes[i].Id = i;
                    //    sb.AppendLine($"{allNotes[i].Id}, {allNotes[i].CreatedAt}, {allNotes[i].Content}, {allNotes[i].Status}");
                    //}

                    //File.WriteAllText(fileName, sb.ToString());

                    Console.WriteLine("Your note DELETE successfully!");
                }

                return;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void DeleteAllNotes()
        {
            try
            {
                Console.Write("Are you sure you want to DELETE all notes? (Y/N): ");
                string confirmAnswer = Console.ReadLine();

                if (confirmAnswer == "Y")
                {
                    allNotes.Clear();

                    File.WriteAllText(fileName, "");
                }

                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void SearchByKeyword()
        {
            Console.WriteLine("Enter content keyword you want to search: ");
            try
            {
                string enteredString = Console.ReadLine().Trim();
                while (true)
                {
                    if (string.IsNullOrEmpty(enteredString))
                    {
                        Console.WriteLine("Your search cannot empty. Please try again!");
                    }
                    else
                    {
                        break;
                    }
                }

                var filteredNotes = allNotes.Where(n => n.Content.Contains(enteredString));

                if (!filteredNotes.Any())
                {
                    Console.WriteLine("No note found.");
                    return;
                }

                foreach (var note in filteredNotes)
                {
                    Console.WriteLine(note.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void WriteToFile()
        {
            string json = JsonSerializer.Serialize(allNotes);
            File.WriteAllText(fileName, json);
        }

        public static void SortNotes()
        {
            string enteredString;
            Console.WriteLine("1. Sort notes by date ascending");
            Console.WriteLine("2. Sort notes by date descending");
            Console.WriteLine("3. Sort notes by status ascending");
            Console.WriteLine("4. Sort notes by status descending");
            Console.WriteLine("Choose an option: ");
            try
            {
                while (true)
                {
                    enteredString = Console.ReadLine();
                    if (string.IsNullOrEmpty(enteredString))
                    {
                        Console.WriteLine("Your option cannot empty, please try again!");
                    }
                    else
                    {
                        break;
                    }
                }
                int.TryParse(enteredString, out int sortOption);

                switch (sortOption)
                {
                    case 1:
                        allNotes = allNotes.OrderBy(n => n.CreatedAt).ToList();
                        WriteToFile();
                        break;
                    case 2:
                        allNotes = allNotes.OrderByDescending(n => n.CreatedAt).ToList();
                        WriteToFile();
                        break;
                    case 3:
                        allNotes = allNotes.OrderBy(n => n.Status).ToList();
                        WriteToFile();
                        break;
                    case 4:
                        allNotes = allNotes.OrderByDescending(n => n.Status).ToList();
                        WriteToFile();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}