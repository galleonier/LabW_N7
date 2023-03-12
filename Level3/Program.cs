using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using Number1;
using System.Diagnostics;

namespace Level3
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Number3.Program.Start();
        }
    }
}

namespace Number1
{
    public static class Program
    {
        /// Результаты сессии содержат оценки 5 экзаменов по каждой группе. Определить средний балл для трех групп студентов одного
        /// потока и выдать список групп в порядке убывания среднего балла. Результаты вывести в виде таблицы с заголовком.

        public static void Start()
        {
            Group[] groups = new Group[3];
            int Ind = 0;
            for (int i = 0; i < groups.Length; i++)
            {
                groups[i] = Group.GroupGenerate(i, 15 + i * 5, (i + 3));
                groups[i].Students = SupportMethods.StudentsListSort(groups[i].Students);
                Ind = SupportMethods.SearchForTheFirstUntested(groups[i].Students);
                groups[i].DeleteBelowIndex(Ind);
                groups[i].RefrashAverMark();
            }
            groups = SupportMethods.GroupsSort(groups);
            Console.WriteLine("Group\tAverage mark");
            foreach (var group in groups)
            {
                Console.WriteLine($"{group.ObjectName}\t{group.AverMark}");
            }
            
            Console.WriteLine(" ");
            foreach (var group in groups)
            {
                SupportMethods.PrintStudents(group);
            }
        }
    }

    public class Base
    {
        public string ObjectName;
        public double AverMark;
    }
    public class Student : Base
    {
        public int[] Marks;
        public Student(string objectname, int[] marks)
        {
            ObjectName = objectname;
            Marks = marks; 
            RefrashAverMark();
        }
        public void RefrashAverMark()
        {
            double sum = 0;
            foreach (int mark in Marks) if (mark >= 3) sum += mark; else {sum = 0; break;} 
            AverMark = sum / Marks.Length;
        }
    }

    public class Group : Base
    {
        public List<Student> Students;
        public Group(string objectName, List<Student> students)
        {
            ObjectName = objectName;
            Students = students;
            RefrashAverMark();
        }

        public void RefrashAverMark()
        {
            double sum = 0;
            foreach (var student in Students) sum += student.AverMark;
            AverMark = sum / Students.Count;
        }

        public static Group GroupGenerate(int GroupNumber, int CountOfStudent, int rang)
        {
            List<Student> students = new List<Student>();
            int stmark = 0;
            for (int i = 0; i < CountOfStudent; i++)
            {
                stmark = (i % 3) + 2;
                students.Add(new Student($"F{i + 1}", new int[5] { rang, 5, stmark, stmark, rang }));
            }
            return new Group($"Group{GroupNumber+1}", students);
        }

        public void DeleteBelowIndex(int Ind)
        {
            for (int j = Ind; j < Students.Count; j++)
            {
                Students.RemoveAt(j);
                j--;
            }
        }
    }

    public static class SupportMethods
    {
        public static Group[] GroupsSort(Group[] groups)
        {
            for (int i = 0; i < groups.Length - 1; i++)
            {
                Group temp;
                for (int j = i + 1; j < groups.Length; j++)
                {
                    if (groups[j].AverMark > groups[i].AverMark)
                    {
                        temp = groups[i];
                        groups[i] = groups[j];
                        groups[j] = temp;
                    }
                }
            }

            return groups;
        }

        public static List<Student> StudentsListSort(List<Student> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            { 
                Student temp; 
                for (int j = i + 1; j < list.Count; j++) 
                { 
                    if (list[j].AverMark > list[i].AverMark) 
                    { 
                        temp = list[i]; 
                        list[i] = list[j]; 
                        list[j] = temp;
                    }
                }
            }
            return list;
        }

        public static void PrintStudents(Group groups)
        {
            Console.WriteLine(groups.ObjectName);
            foreach (var student in groups.Students)
            {
                Console.WriteLine($"{student.ObjectName}\tAverage mark: {student.AverMark}");
            }
            Console.WriteLine();
        }

        public static int SearchForTheFirstUntested(List<Student> list)
        {
            int low = 0;
            int high = list.Count - 1;
            int Ind = 0;
            while (list[Ind].AverMark!=0)
            {
                Ind = (low + high) / 2;
                if (0 > list[Ind].AverMark) high = Ind - 1;
                else if (0 < list[Ind].AverMark) low = Ind + 1;
            } 
            while (list[Ind].AverMark == 0) Ind--;
            return (Ind+1);
        }
        
    }
}

namespace Number3
{
    public static class Program
    {
        /// В соревнованиях участвуют три команды по 6 человек. Результаты соревнований представлены в виде мест участников каждой команды (1 – 18).
        /// Определить команду-победителя, вычислив количество баллов, набранное каждой командой. Участнику, занявшему 1-е место, начисляется 5 баллов,
        /// 2-е – 4, 3-е – 3, 4-е – 2, 5-е – 1, остальным – 0 баллов. При равенстве баллов победителем считается команда, за которую выступает участник, занявший 1-е место.
        public static void Start()
        {
            int[] Mas = new int[] { 7, 13, 2, 17, 1, 11, 10, 9, 14, 4, 18, 8, 3, 15, 5, 16, 6, 12 };
            Group[] groups = new Group[3];
            for (int i = 0; i < groups.Length; i++)
            {
                groups[i] = Group.GroupGenerate(i, 6, Mas, i );
            }
            groups = SupportMethods.GroupsSort(groups);
            Console.WriteLine("Group\tAverage mark");
            foreach (var group in groups)
            {
                Console.WriteLine($"{group.ObjectName}\t{group.Result}");
            }

        }
    }

    public class Player
    {
        public string ObjectName;
        public int Result;
    }
    public class Group : Player
    {
        public List<Player> Players;
        public int BestResult; 
        public Group(string objectName, List<Player> players)
        {
            ObjectName = objectName;
            Players = players;
            BestResult = players[0].Result;
            foreach (var player in Players) 
            {
                if (player.Result < 6) Result+=(6-player.Result);
                if (player.Result < BestResult) BestResult = player.Result;
            }
            
        }
        public static Group GroupGenerate(int GroupNumber, int CountOfPlayers, int[] Mas, int Rang)
        {
            List<Player> players = new List<Player>();
            for (int i = 0; i < CountOfPlayers; i++)
            {
                Player Pl = new Player();
                Pl.ObjectName = $"Player{i}";
                Pl.Result = Mas[(Rang * 6) + i];
                players.Add(Pl);
            }
            return new Group($"Group{GroupNumber+1}", players);
        }
    }

    public static class SupportMethods
    {
        public static Group[] GroupsSort(Group[] groups)
        {
            for (int i = 0; i < groups.Length - 1; i++)
            {
                Group temp;
                for (int j = i + 1; j < groups.Length; j++)
                {
                    if (groups[j].Result > groups[i].Result)
                    {
                        temp = groups[i];
                        groups[i] = groups[j];
                        groups[j] = temp;
                    }
                    else if (groups[j].Result > groups[i].Result || groups[j].BestResult > groups[i].BestResult)
                    { 
                        temp = groups[i]; 
                        groups[i] = groups[j]; 
                        groups[j] = temp;
                    }
                }
            }
            return groups;
        }

    }
}

