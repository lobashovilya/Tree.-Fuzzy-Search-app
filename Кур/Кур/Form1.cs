using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Кур
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            using (StreamReader stream = new StreamReader("slovar.txt"))
            {
                while (stream.Peek() >= 0)
                {
                    string str = stream.ReadLine();
                    string[] tmp = str.Split(',');
                    slovar = tmp;
                }
            }
            foreach (string word in slovar)
            {
                textBox3.Text += word + ", ";
            }
            textBox3.Text = textBox3.Text.Remove(textBox3.Text.Length - 1);
            textBox3.Text = textBox3.Text.Remove(textBox3.Text.Length - 1);
            textBox3.Text += ".";
        }
        string[] slovar;
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public class Tree
        {
            public class TreeNode
            {
                private readonly Dictionary<char, TreeNode> subNode; // подузел
                private bool isEnd; // конец слова
                private readonly string word; // слово
                public TreeNode(string entering_word)
                {
                    subNode = new Dictionary<char, TreeNode>();
                    isEnd = false;
                    word = entering_word;
                }
                public Dictionary<char, TreeNode> SubNode { get { return subNode; } }
                public bool IsEnd { get { return isEnd; } set { isEnd = value; } }
                public string Word { get { return word; } }
            }

                public TreeNode root;
                public Tree()
                {
                    root = new TreeNode(String.Empty);
                }
                public void Add(string word)
                {
                    AddRecursive(root, word, String.Empty);
                }
                public static void AddRecursive(TreeNode node, string entering_string, string current_string)
                {
                    if (entering_string.Length == 0) { return; }
                    char prefix = entering_string[0];
                    string substring = entering_string.Substring(1);
                    if (!node.SubNode.ContainsKey(prefix))
                    {
                        node.SubNode.Add(prefix, new TreeNode(current_string + prefix));
                    }
                    if (substring.Length == 0)
                    {
                        node.SubNode[prefix].IsEnd = true;
                        return;
                    }
                    else
                    {
                        AddRecursive(node.SubNode[prefix], substring, current_string + prefix);
                    }
                }
                public IEnumerable<string> Find (string find_word)
                {
                    TreeNode node = root;
                    foreach (var find in find_word)
                    {
                        if (!node.SubNode.ContainsKey(find))
                        {
                            return new string[0];
                        }
                        node = node.SubNode[find];
                    }
                    return Checkword(node);
                }
                public IEnumerable<string> Checkword (TreeNode node)
                {
                    if (node.IsEnd)
                    {
                        yield return node.Word;
                    }

                foreach (var subnode in node.SubNode)
                {
                    foreach (var result in Checkword(subnode.Value))
                    {
                        yield return result;
                    }
                }
                }
            public IEnumerable<string> Obhod()
            {
                TreeNode node = root;
                return Checkword(node);
            }  

        }
        public static int rasdamlen(string first, string second)
        {
            if (string.IsNullOrEmpty(first))
                if (string.IsNullOrEmpty(second))
                    return 0;
                else
                    return second.Length;
            else if (string.IsNullOrEmpty(second))
                return first.Length;

            if (second.Length > first.Length)
            {
                string tmp = second;
                second = first;
                first = tmp;
            }

            int[] firstDp = new int[second.Length + 1];
            int[] secondDp = new int[second.Length + 1];
            int[] thirdDp = new int[second.Length + 1];
            int inf = first.Length + second.Length;

            for (int j = 0; j <= second.Length; ++j)
                thirdDp[j] = j;

            for (int i = 1; i <= first.Length; ++i)
            {
                int[] tmp = firstDp;
                firstDp = secondDp;
                secondDp = thirdDp;
                thirdDp = tmp;
                thirdDp[0] = secondDp[0] + 1;
                for (int j = 1; j <= second.Length; ++j)
                {
                    int[] dist = new int[] {
                        secondDp[j] + 1,
                        thirdDp[j - 1] + 1,
                        secondDp[j - 1] + (first[i - 1] != second[j - 1] ? 1 : 0),
                        i > 1 && j > 1 ? firstDp[j - 2] + (first[i - 1] == second[j - 2] && first[i - 2] == second[j - 1] ? 1 : inf) : inf
                    };
                    thirdDp[j] = dist.Min();
                }
            }
            return thirdDp[second.Length];
        }

        public double pohoj (string etalon, string srav)
        {
            int k = etalon.Length + srav.Length;
            int c = 0;
            for (int i = 0; i < etalon.Length; i++)
                for (int j = 0; j < srav.Length; j++)
                {
                    if (etalon[i] == srav[j])
                    {
                        srav = srav.Remove(j, 1);
                        srav = srav.Insert(j, " ");
                        c++;
                    }
                }
            double x;
            return x = (double)c / (double)(k - c);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            label2.Visible = false;
            textBox2.Visible = false;
            comboBox1.Items.Clear();
            comboBox1.Visible = false;
            Queue<string> queue = new Queue<string>();
            Queue<string> queue1 = new Queue<string>();
            textBox2.Clear();
            if (textBox1.Text == String.Empty)
            { MessageBox.Show("Вы ничего не ввели!"); return; }
            string search_word = textBox1.Text.ToLower();
            bool flag = false;
            foreach(char simb in search_word)
            {
                if ((simb >= 'а') && (simb <= 'я'))
                { flag = true; }
            }
            if (flag)
            { MessageBox.Show("Можно вводить только латинские буквы!"); textBox1.Clear();    return; }
            try
            {
                int stroka = Convert.ToInt32(search_word);
                MessageBox.Show("Вы ввели не слово!");
                textBox1.Clear();
                return;
            }
            catch {}
            textBox1.Text = textBox1.Text.ToLower();
            Tree trie = new Tree();
            foreach (string word in slovar)
            {
                trie.Add(word);
            }
            var derevo = trie.Obhod();
            var result = trie.Find(search_word);
            if (result.Count() == 0)
            {
                foreach (string word in derevo)
                {
                    if (rasdamlen(word, search_word) < 4)
                    { queue.Enqueue(word); }
                }
            }
            else
            {
                foreach (var word in result)
                {
                    if (rasdamlen(word, search_word) < 4)
                    { queue.Enqueue(word); }
                }
            }
            if (queue.Count == 0)
            { MessageBox.Show("Вы сделали слишком много ошибок!"); textBox1.Clear(); return; }
            double x = 0;
            while (queue.Count != 0)
            {
               double y = pohoj(search_word, queue.Peek());
                if (x<y)
               {
                    queue1.Clear();
                    x = y;
                    queue1.Enqueue(queue.Peek());
               }
                else if (x == y)
                { queue1.Enqueue(queue.Peek()); }
                   queue.Dequeue();
            }
            if (queue1.Count > 1)
            {
                label2.Visible = true;
                comboBox1.Visible = true;
                foreach (var word in queue1)
                {
                    comboBox1.Items.Add(word);
                }
            }
            else
            {
                textBox2.Visible = true;
                textBox2.Text = queue1.Peek();
            }
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label2.Visible = false;
            textBox2.Visible = true;
            textBox2.Text = Convert.ToString(comboBox1.SelectedItem);
            comboBox1.Visible = false;
        }
    }
 }
