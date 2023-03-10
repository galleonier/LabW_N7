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
            Number1.Program.Start();
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

namespace Number2
{
 public static class Program
    {
        /// Лыжные гонки проводятся отдельно для двух групп участников. Результаты соревнований заданы в виде фамилий участников и
        /// их результатов в каждой группе. Расположить результаты соревнований в каждой группе в порядке занятых мест.
        /// Объединить результаты обеих групп с сохранением упорядоченности и вывести в виде таблицы с заголовком.

        public static void Start()
        {
            List<Participant> TotalResult = new List<Participant>();
            List<Participant>[] groups = new List<Participant>[2];
            for (int i = 0; i < groups.Length; i++)
            {
                groups[i] = SupportMethods.GroupGenerate(i, 15 + i * 5, i+1);
                groups[i] = SupportMethods.ParticipantListSort(groups[i]);
                SupportMethods.PrintParticipant(groups[i]);
            }
            TotalResult = new List<Participant>(SupportMethods.CombiningTheResult(groups[0], groups[1]));
            SupportMethods.PrintParticipantTable(TotalResult);
        }
    }

    public struct Participant
    {
        public string Lastname;
        public string GroupName;
        public int Result;
        public Participant(string lastname, string groupname, int result)
        {
            Lastname = lastname;
            Result = result;
            GroupName = groupname;
        }
    }

    public static class SupportMethods
    {
        public static List<Participant> ParticipantListSort(List<Participant> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            { 
                Participant temp; 
                for (int j = i + 1; j < list.Count; j++) 
                { 
                    if (list[j].Result > list[i].Result) 
                    { 
                        temp = list[i]; 
                        list[i] = list[j]; 
                        list[j] = temp;
                    }
                }
            }
            return list;
        }
        public static List<Participant> CombiningTheResult(List<Participant> list1, List<Participant> list2)
        {
            List<Participant> fin = new List<Participant>();
            while (list1.Count > 0 & list2.Count > 0)
            {
                if (list1.Count == 0) fin.Add(list2[0]);
                else if (list2.Count == 0)
                    fin.Add(list1[0]);
                else if (list1[0].Result < list2[0].Result)
                {
                    fin.Add(list2[0]);
                    list2.RemoveAt(0);
                }
                else if (list1[0].Result > list2[0].Result)
                {
                    fin.Add(list1[0]);
                    list1.RemoveAt(0);
                }
                else
                {
                    fin.Add(list1[0]);
                    list1.RemoveAt(0);
                    fin.Add(list2[0]);
                    list2.RemoveAt(0);
                }
            }
            return fin;
        }
        public static void PrintParticipant(List<Participant> group)
        {
            Console.WriteLine(group[0].GroupName);
            foreach (var participant in group)
            {
                Console.WriteLine($"{participant.Lastname}\tResult: {participant.Result}");
            }
            Console.WriteLine();
        }
        public static void PrintParticipantTable(List<Participant> group)
        {
            foreach (var participant in group)
            {
                Console.WriteLine($"{participant.GroupName}\t{participant.Lastname}\tResult: {participant.Result}");
            }
            Console.WriteLine();
        }
        public static List<Participant> GroupGenerate(int GroupNumber, int CountOfParticipants, int rang)
        {
            List<Participant> participants = new List<Participant>();
            int stmark = 0;
            for (int i = 0; i < CountOfParticipants; i++)
            {
                stmark = (i % 3) + 2;
                participants.Add(new Participant($"F{i + 1}",$"Group{GroupNumber+1}",  ((i+1)*2)*((i%3)+1)));
            }
            return participants;
        }
    }
}

/* 
2. Соревнования по футболу между командами проводятся в два
этапа. Для проведения первого этапа участники разбиваются на две
группы по 12 команд. Для проведения второго этапа выбирается
6 лучших команд каждой группы по результатам первого этапа. Составить список команд участников второго этапа.
3. В соревнованиях участвуют три команды по 6 человек. Результаты соревнований представлены в виде мест участников каждой команды (1 – 18). Определить команду-победителя, вычислив количество баллов, набранное каждой командой. Участнику, занявшему 1-е
место, начисляется 5 баллов, 2-е – 4, 3-е – 3, 4-е – 2, 5-е – 1, остальным – 0 баллов. При равенстве баллов победителем считается команда, за которую выступает участник, занявший 1-е место.
4. Лыжные гонки проводятся отдельно для двух групп участников. Результаты соревнований заданы в виде фамилий участников и
их результатов в каждой группе. Расположить результаты соревнований в каждой группе в порядке занятых мест. Объединить результаты
обеих групп с сохранением упорядоченности и вывести в виде таблицы с заголовком.
5. Обработать результаты первенства по футболу. Результаты каждой игры заданы в виде названий команд и счета (количество забитых
и пропущенных мячей). Сформировать таблицу очков (выигрыш – 3,
178
ничья – 1, проигрыш – 0) и упорядочить результаты в соответствии с
занятым местом. Если сумма очков у двух команд одинакова, то сравниваются разности забитых и пропущенных мячей. Вывести результирующую таблицу, содержащую место, название команды, количество очков.
6. Японская радиокомпания провела опрос радиослушателей по
трем вопросам:
а) какое животное вы связываете с Японией и японцами?
б) какая черта характера присуща японцам больше всего?
в) какой неодушевленный предмет или понятие вы связываете с
Японией?
Большинство опрошенных прислали ответы на все или часть вопросов. Составить программу получения первых пяти наиболее часто
встречающихся ответов по каждому вопросу и доли (%) каждого такого ответа. Предусмотреть необходимость сжатия столбца ответов в
случае отсутствия ответов на некоторые вопросы.*/