using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Code_Converter
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        public struct arrangement
        {
            public string sub_;
            public string func_;
            public string class_;

            public string def_;
            public string defvalue_;
            public string set_;

            public string defAM_;
            public string defvalueAM_;

            public string if_;
            public string while_;

            public string subNoAM_;
            public string funcNoAM_;
            public string classNoAM_;

            public string endStatement_;

            // for

            public arrangement(string _sub, string _func, string _class, string _def, string _set, string _defvalue, string _if, string _while, string _defAM, string _defvalueAM, string _subNoAM, string _funcNoAM, string _classNoAM, string _endStatement)
            {
                sub_ = _sub;
                func_ = _func;
                class_ = _class;

                def_ = _def;
                defvalue_ = _defvalue;
                set_ = _set;

                if_ = _if;
                while_ = _while;

                defAM_ = _defAM;
                defvalueAM_ = _defvalueAM;

                subNoAM_ = _subNoAM;
                funcNoAM_ = _funcNoAM;
                classNoAM_ = _classNoAM;

                endStatement_ = _endStatement;
            }
        }

        public class Language
        {
            public string name = "";

            public string lineChar = "";

            public List<string> keywords = new List<string>();

            public arrangement setup;

            public Language(string _name, string _lineChar, List<string> _keywords, arrangement _setup)
            {
                name = _name;
                lineChar = _lineChar;

                keywords = _keywords;

                setup = _setup;
            }

        }

        public string ReplaceWord(Language original, Language converted, string word)
        {
            int pos = original.keywords.IndexOf(word);
            return converted.keywords[pos] + ' '; ;
        }

        public string ReplaceReg(string toReplace, string with,string text)
        {
            string output = "";
            string[] lines = text.Split(new string[] { toReplace }, StringSplitOptions.None);
            foreach(string line in lines)
            {
                output = output + line + with + ' ';
            }
            output = output.Substring(0, (output.Length - with.Length)-1);
            return output;
        }

        public string Combine(string text, string[] args)
        {
            string output = "";

            string[] bits = text.Split('{');
            foreach (string bit in bits)
            {
                if (bit.Length >= 2)
                {
                    if (bit != "" && bit[1] == '}' && args.Length > int.Parse(bit[0].ToString()))
                    {
                        output = output + args[int.Parse(bit[0].ToString())] + bit.Substring(2);
                    }
                    else
                    {
                        if (bit[1] == '}')
                        {
                            output = output + '{' + bit;
                        }
                        else
                        {
                            output = output + bit;
                        }
                    }
                }
            }



            return output;
        }

        public string[] cleanList(string[] list, string removable)
        {
            List<string> listVersion = new List<string>();

            foreach(string item in list)
            {
                if(item != removable)
                {
                    listVersion.Add(item);
                }
            }

            return listVersion.ToArray();

        }

        public string getDataFromTemplate(string template, int num, string text, bool _void)
        {
            string output = "";

            if (text != "")
            {
                string[] nums = template.Split(' ');

                int counter = 0;

                foreach (string bit in nums)
                {
                    if (bit.Length >= 2)
                    {
                        if (bit[0] == '{' && bit[2] == '}')
                        {
                            if (int.Parse(bit[1].ToString()) == num)
                            {
                                break;
                            }
                        }
                    }
                    if (_void)
                    {
                        counter++;
                    }
                    if(!_void && bit != "void")
                    {
                        counter++;
                    }
                }

                string[] words = text.Split(' ');

                words = cleanList(words, "");

                output = words[counter];
            }
            else
            {
                output = text;
            }
            
            return output;
        }

        public string getAMsFinalIndex(string data)
        {
            bool ended = false;
            bool started = false;
            int start = 0;
            int end = 0;
            int counter = 0;

            string[] bits = data.Split(' ');
            foreach(string bit in bits)
            {
                if (isAM(bit))
                {
                    if (!started)
                    {
                        start = counter;
                        started = true;
                    }
                }
                else
                {
                    if (!ended && started)
                    {
                        ended = true;
                        end = counter;
                    }
                }
                counter = counter + bit.Length + 1;
            }

            string output = start.ToString() + ' ' + end.ToString();

            return output;
        }

        public bool isAM(string data)
        {
            List<string> AMs = new List<string>() { "public","static" };

            string[] bits = data.Split(' ');
            foreach(string bit in bits)
            {
                if (AMs.Contains(bit))
                {
                    return true;
                }
            }
            return false;
        }

        public string connector(string text,string seporator,string connector)
        {
            string output = "";
            string[] lines = text.Split(new string[] { seporator }, StringSplitOptions.None);
            foreach(string line in lines)
            {
                if(line != "")
                    output = output + line + connector;
            }
            output = output.Substring(0, (output.Length - connector.Length));
            return output;
        }

        public bool returnType(string text, bool void_)
        {
            text = text.ToLower();

            List<string> returnTypes = new List<string>() { "void", "string", "int" }; ;

            if (!void_)
                returnTypes.Remove("void");

            foreach (string returnType in returnTypes)
            {
                if (text.Contains(returnType.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        public bool correctBits(string type, string line, Language old) // current work
        {
            if (type == "Function")
            {
                if (line.Contains('(') && line.Contains(')') && returnType(line, false) && !line.Contains('='))
                {
                    return true;
                }
            }

            if (type == "ASG")
            {
                if (returnType(line, false) && line.Contains(old.keywords[Cb.keywords.IndexOf("ASG")]))
                {
                    return true;
                }
            }

            if (type == "Def")
            {
                if (returnType(line, false))
                {
                    return true;
                }
            }

            if(type == "VAL")
            {
                if (line.Contains(old.keywords[Cb.keywords.IndexOf("VAL")]))
                {
                    return true;
                }
            }

            if(type == "Sub")
            {
                if (line.Contains('(') && line.Contains(')') && !line.Contains('='))
                {
                    bool containsVoid = line.Contains(old.keywords[Cb.keywords.IndexOf("Void")]);

                    if (containsVoid || isAM(line) && !containsVoid)
                    {
                        return true;
                    }
                }
            }


            return false;
        }

        public string TABS(string line)
        {
            string[] bits = line.Split('\t');

            string result = "";

            for (int i = bits.Length-1; i >= 0; i--)
            {
                if(i != 0)
                {
                    result = result + '\t';
                }
            }

            return result;
        }

        public string findLastStatementOpener(Language newLang, int[] Indexs)
        {
            List<string> RevisedIndexes = new List<string>();

            foreach(int index in Indexs)
            {
                RevisedIndexes.Add(newLang.keywords[(index)]);
            }


            string text = Output.Text;

            string[] lines = text.Split(new string[] { "\n", "\r", "\r\n" }, StringSplitOptions.None);

            int found = -1;

            int counter = 0;

            while (found<0)
            {

                for (int i = lines.Length - 1; i >= 0; i--)
                {
                    string line = lines[i];

                    if (line != "")
                    {

                        // if while else for foreach

                        counter = 0;

                        foreach (string index in RevisedIndexes)
                        {
                            if (line.Contains(index))
                            {
                                found = counter;
                            }
                            counter++;
                        }
                    }
                }
            }

            return RevisedIndexes[found];
        }

        public int firstOfChar(string line, Char findable)
        {
            int counter = 0;

            foreach(Char letter in line)
            {
                if(letter == findable)
                {
                    return counter;
                }
                counter++;
            }
            return 0;
        }

        public string getValue(string text, string templateOld, int num, Language old)
        {
            int orderNumber = text.IndexOf(getDataFromTemplate(templateOld, num, text, true));

            string newText = text.Substring(orderNumber);

            //TODO: finish function NOT NESSISARY

            string[] bits = templateOld.Split(new string[] { "{" + num.ToString() + "}" }, StringSplitOptions.None);
            string bit = bits[1];

            return newText;
        }

        public string Handler(string line, Language _old, Language _new)
        {
            string newLine = "";

            bool used = false;

            string template = "";
            string templateOld = "";

            string name = "";
            string type = "";
            string value = "";
            string condition = "";

            string statementType = "";

            string returnType = "";
            string parameters = "";

            int define = Cb.keywords.IndexOf("Def");
            int defineandset = Cb.keywords.IndexOf("ASG");
            int set = Cb.keywords.IndexOf("VAL");

            int ifStatement = Cb.keywords.IndexOf("If");
            int whileStatement = Cb.keywords.IndexOf("While");

            int funcStatement = Cb.keywords.IndexOf("Function");
            int subStatement = Cb.keywords.IndexOf("Sub");
            int classesStatement = Cb.keywords.IndexOf("Class");

            int endStatement = Cb.keywords.IndexOf("End");

            string numberOfTabs = TABS(line);

            #region End

            if (endStatement != -1) // if not in old template
            {
                if (line.Contains(_old.keywords[endStatement]) && used == false)
                {
                    template = _new.setup.endStatement_;
                    templateOld = _old.setup.endStatement_;

                    if (template.Contains("{0}"))
                    {
                        if (templateOld.Contains("{0}"))
                        {
                            statementType = getDataFromTemplate(templateOld, 0, line,true);
                        }
                        else
                        {
                            int elseStatement = Cb.keywords.IndexOf("Else");

                            statementType = findLastStatementOpener(_new, new int[] { ifStatement, whileStatement, funcStatement, subStatement, classesStatement,elseStatement });
                        }

                        newLine = newLine + Combine(template, new string[] { statementType });
                    }
                    else
                    {
                        newLine = template;
                    }

                    used = true;
                }
            }

            #endregion

            #region If

            if (ifStatement != -1)
            {
                if (line.Contains(_old.keywords[ifStatement]) && used == false)
                {
                    template = _new.setup.if_;
                    templateOld = _old.setup.if_;

                    condition = getDataFromTemplate(templateOld, 0, line,true);

                    newLine = newLine + Combine(template, new string[] { condition });

                    used = true;
                }
            }

            #endregion

            #region While

            if (whileStatement != -1)
            {
                if (line.Contains(_old.keywords[whileStatement]) && used == false)
                {
                    template = _new.setup.while_;
                    templateOld = _old.setup.while_;

                    condition = getDataFromTemplate(templateOld, 0, line,true);

                    newLine = newLine + Combine(template, new string[] { condition });

                    used = true;
                }
            }

            #endregion

            #region Classes

            if (classesStatement != -1)
            {
                if (line.Contains(_old.keywords[classesStatement]) && used == false)
                {
                    template = _new.setup.classNoAM_;
                    templateOld = _old.setup.classNoAM_;

                    bool hasAM = isAM(line);

                    if (hasAM)
                    {
                        template = _new.setup.class_;
                        templateOld = _old.setup.class_;

                        // access modifyers
                        string[] amPoints = getAMsFinalIndex(line).Split(' ');
                        int start = int.Parse(amPoints[0]);
                        string am = line.Substring(start, int.Parse(amPoints[1]) - start);
                        string finishedAm = connector(am, " ", "$$$$");

                        line = ReplaceReg(am, finishedAm, line);
                        //MessageBox.Show(line);

                        name = getDataFromTemplate(templateOld, 1, line,true);

                        newLine = newLine + Combine(template, new string[] { am, name });
                        //MessageBox.Show(newLine+ " here");


                    }
                    else
                    {
                        // no access modifyers
                        name = getDataFromTemplate(templateOld, 0, line,true);

                        newLine = newLine + Combine(template, new string[] { name });
                    }
                    used = true;
                }
            }


            #endregion

            #region Functions

            string parametersEdited = "";
            string editedline = "";
            string lineRedone = line;

            int opener = firstOfChar(line, '('), closer = firstOfChar(line, ')');
            if (opener != 0 && closer != 0)
            {
                editedline = line.Substring(0, opener) + ' ' + line.Substring(closer+1);
                parametersEdited = line.Substring(opener + 1, closer - opener - 1);
                parametersEdited = Localise(parametersEdited, _old, _new);
                parametersEdited = parametersEdited.Insert(0, "(");
                parametersEdited = parametersEdited + ')';

                string temporaryHolder = "";

                string[] parts = parametersEdited.Split(',');
                if(parts.Length > 1)
                {
                    foreach(string part in parts)
                    {
                        if(part[0] != ' ')
                        {
                            temporaryHolder = temporaryHolder + ' ' + part + ',';
                        }
                        else
                        {
                            temporaryHolder = temporaryHolder + part + ',';
                        }
                    }

                    temporaryHolder = temporaryHolder.Substring(0, temporaryHolder.Length - 1);
                }
                if(parts.Length == 1)
                {
                    temporaryHolder = parametersEdited;
                }
                if(parts.Length == 0)
                {
                    temporaryHolder = parametersEdited;
                }

                //space issue (clean up)

                parametersEdited = temporaryHolder;

                editedline = editedline + "()";
                lineRedone = lineRedone.Substring(0, opener) + line.Substring(closer+1);
                //MessageBox.Show("redone - "+lineRedone);
                //MessageBox.Show("editeded - "+editedline);
            }

            if (correctBits("Function", editedline, _old))
            {
                #region Func

                if (funcStatement != -1)
                {
                    if (line.Contains(_old.keywords[funcStatement]) && used == false)
                    {
                        line = lineRedone;

                        template = _new.setup.funcNoAM_;
                        templateOld = _old.setup.funcNoAM_;

                        bool hasAM = isAM(line);

                        if (hasAM)
                        {
                            template = _new.setup.func_;
                            templateOld = _old.setup.func_;

                            // access modifyers
                            string[] amPoints = getAMsFinalIndex(line).Split(' ');
                            int start = int.Parse(amPoints[0]);
                            string am = line.Substring(start, int.Parse(amPoints[1]) - start);
                            string finishedAm = connector(am, " ", "$$$$");

                            line = ReplaceReg(am, finishedAm, line);
                            //MessageBox.Show(line);

                            name = getDataFromTemplate(templateOld, 1, line, true);
                            //parameters = getDataFromTemplate(templateOld, 2, line, true);
                            returnType = getDataFromTemplate(templateOld, 3, line, true);

                            newLine = newLine + Combine(template, new string[] { am, name, parameters, returnType });
                            //MessageBox.Show(newLine+ " here");


                        }
                        else
                        {
                            // no access modifyers
                            name = getDataFromTemplate(templateOld, 0, line, true);
                            //parameters = getDataFromTemplate(templateOld, 1, line, true);
                            returnType = getDataFromTemplate(templateOld, 2, line, true);

                            newLine = newLine + Combine(template, new string[] { name, parameters, returnType });
                        }
                        used = true;
                    }
                }


                #endregion
            }

            if (correctBits("Sub", editedline, _old))
            {
                #region Sub

                bool hasvoid = line.Contains(_old.keywords[Cb.keywords.IndexOf("Void")]);

                if (subStatement != -1)
                {
                    if (line.Contains(_old.keywords[subStatement]) && used == false)
                    {
                        line = lineRedone;

                        template = _new.setup.subNoAM_;
                        templateOld = _old.setup.subNoAM_;

                        bool hasAM = isAM(line);

                        if (hasAM)
                        {
                            template = _new.setup.sub_;
                            templateOld = _old.setup.sub_;

                            // access modifyers
                            string[] amPoints = getAMsFinalIndex(line).Split(' ');
                            int start = int.Parse(amPoints[0]);
                            string am = line.Substring(start, int.Parse(amPoints[1]) - start);
                            string finishedAm = connector(am, " ", "$$$$");

                            line = ReplaceReg(am, finishedAm, line);
                            //MessageBox.Show(line);

                            name = getDataFromTemplate(templateOld, 1, line,hasvoid);
                            //parameters = getDataFromTemplate(templateOld, 2, line,hasvoid);

                            newLine = newLine + Combine(template, new string[] { am, name, parameters });
                            //MessageBox.Show(newLine+ " here");


                        }
                        else
                        {
                            // no access modifyers
                            name = getDataFromTemplate(templateOld, 0, line,true);
                            //parameters = getDataFromTemplate(templateOld, 1, line,true);

                            newLine = newLine + Combine(template, new string[] { name, parameters });
                        }
                        used = true;
                    }
                }

                #endregion
            }

            if (name != "")
            {
                int pos = newLine.IndexOf(name);

                opener = pos + name.Length + 1;
                newLine = newLine.Insert(opener, parametersEdited);
            }

            #endregion

            #region For

            #endregion

            #region Variables
            
            if (correctBits("ASG", line, _old))
            {

                #region Define And Set

                if (defineandset != -1)
                {
                    if (line.Contains(_old.keywords[defineandset]) && !used)
                    {
                        template = _new.setup.defvalue_;
                        templateOld = _old.setup.defvalue_;

                        bool hasAM = isAM(line);

                        if (hasAM)
                        {
                            template = _new.setup.defvalueAM_;
                            templateOld = _old.setup.defvalueAM_;

                            // access modifyers
                            string[] amPoints = getAMsFinalIndex(line).Split(' ');
                            int start = int.Parse(amPoints[0]);
                            string am = line.Substring(start, int.Parse(amPoints[1]) - start);
                            string finishedAm = connector(am, " ", "$$$$");

                            line = ReplaceReg(am, finishedAm, line);
                            //MessageBox.Show(line);

                            type = getDataFromTemplate(templateOld, 1, line,true);
                            name = getDataFromTemplate(templateOld, 2, line,true);
                            value = getValue(line, templateOld, 3, _old);
                            //value = getDataFromTemplate(templateOld, 3, line,true);

                            newLine = newLine + Combine(template, new string[] { am, type, name, value });
                            //MessageBox.Show(newLine + " here");


                        }
                        else
                        {
                            // no access modifyers
                            type = getDataFromTemplate(templateOld, 0, line,true);
                            name = getDataFromTemplate(templateOld, 1, line,true);
                            value = getValue(line, templateOld, 2, _old);
                            //value = getDataFromTemplate(templateOld, 2, line,true);

                            newLine = newLine + Combine(template, new string[] { type, name, value });
                        }
                        used = true;
                    }
                }

                #endregion

            }

            if (correctBits("Def", line, _old))
            {

                #region Define

                if (define != -1)
                {
                    if (line.Contains(_old.keywords[define]) && used == false)
                    {
                        template = _new.setup.def_;
                        templateOld = _old.setup.def_;

                        bool hasAM = isAM(line);

                        if (hasAM)
                        {
                            template = _new.setup.defAM_;
                            templateOld = _old.setup.defAM_;

                            // access modifyers
                            string[] amPoints = getAMsFinalIndex(line).Split(' ');
                            int start = int.Parse(amPoints[0]);
                            string am = line.Substring(start, int.Parse(amPoints[1]) - start);
                            string finishedAm = connector(am, " ", "$$$$");

                            line = ReplaceReg(am, finishedAm, line);
                            //MessageBox.Show(line);

                            type = getDataFromTemplate(templateOld, 1, line,true);
                            name = getDataFromTemplate(templateOld, 2, line,true);

                            newLine = newLine + Combine(template, new string[] { am, type, name });
                            //MessageBox.Show(newLine+ " here");


                        }
                        else
                        {
                            // no access modifyers
                            type = getDataFromTemplate(templateOld, 0, line,true);
                            name = getDataFromTemplate(templateOld, 1, line,true);

                            newLine = newLine + Combine(template, new string[] { type, name });
                        }
                        used = true;
                    }
                }

                #endregion

            }

            if (correctBits("VAL", line, _old))
            {

                #region Set

                if (set != -1)
                {
                    if (line.Contains(_old.keywords[set]) && used == false)
                    {
                        template = _new.setup.set_;
                        templateOld = _old.setup.set_;

                        name = getDataFromTemplate(templateOld, 0, line, true);
                        value = getValue(line, templateOld, 1, _old);

                        //name = (line.Split(new string[] { _old.keywords[set] }, StringSplitOptions.None))[0];
                        //value = (line.Split(new string[] { _old.keywords[set] }, StringSplitOptions.None))[1];

                        newLine = newLine + Combine(template, new string[] { name, value });

                        used = true;
                    }
                }

                #endregion

            }

            #endregion

            if(used == false)
            {
                newLine = line;
            }
            else
            {
                if(numberOfTabs != "")
                {
                    newLine = numberOfTabs + newLine;
                }
            }

            if (newLine != "")
                return Localise(newLine, _old, _new);
            else
                return newLine;
        }
           
        public string Localise(string line, Language from, Language to)
        {
            string newLine = "";

            string[] words = line.Split(' ');
            foreach(string word in words)
            {
                if (from.keywords.Contains(word) && word != "")
                {
                    newLine = newLine + ReplaceWord(from, to, word);
                }
                else
                {
                    newLine = newLine + word + ' ';
                }
            }

            return newLine;
        }

        public string conversion(Language _old, Language _new, string text)
        {
            string[] lines = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            float line_workload = 40 / lines.Length;

            text = "";

            foreach (string line in lines)
            {
                if (line != "")
                    text = text + Handler(line, _old, _new) + Environment.NewLine;
                else
                    text = text + "" + Environment.NewLine;

                Output.Text = text;

                progressBar.Value = progressBar.Value + int.Parse(line_workload.ToString());
            }

            return text;
        }

        public string removeChar(char c, string line)
        {
            string output = "";

            foreach(Char a in line)
            {
                if(a != c)
                {
                    output = output + a;
                }
            }

            return output;

        }

        public string removeLineEndings(string text, Language lang)
        {
            string output = "";


            string[] lines = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach(string line in lines)
            {
                if (line.Contains(lang.lineChar)) // "" error
                {
                    output = output + removeChar(lang.lineChar[0], line) + Environment.NewLine;
                }
                else
                {
                    output = output + line + Environment.NewLine;
                }
            }

            return output;


        }

        public string Convert(Language original, Language converted, string text)
        {
            progressBar.Value = 0;

            string result = "";

            if (original.lineChar != "")
            {
                text = removeLineEndings(text, original);
            }

            progressBar.Value = 20;

            if (original != Cb)
            {
                result = conversion(original, Cb, text);
            }

            progressBar.Value = 60;

            if (converted != Cb)
            {
                result = conversion(Cb, converted, result);
            }

            progressBar.Value = 100;

            return result;
        }

        Language Cs;
        Language VB;
        Language PseudoCode;
        Language Cb;

        List<Language> languages = new List<Language>();

        CustomProgressBar progressBar;

        private void Form_Load(object sender, EventArgs e)
        {
            Color backColor = Color.FromArgb(36, 36, 36);
            Color frontColor = Color.FromArgb(30, 30, 30);
            Color textColor = Color.White;
            Color barColor = Color.FromArgb(0, 255, 0);

            Control.BackColor = frontColor;

            progressBar = new CustomProgressBar();
            progressBar.foreColor = barColor;
            progressBar.backColor = backColor;
            progressBar.Visable = true;
            progressBar.Location = new Point(10, 48);
            progressBar.Size = new Size(1102, 13);

            string variables = "public static int x;\npublic static int x = 5;\npublic int x;\npublic int x = 5;\nint x;\nint x = 5;";
            string whileandif = "if ( true ) {\nwhile ( true ) {";
            string function = "public static string x () {";
            string sub = "public static void x () {";
            string clas = "class x {";
            string end = "if ( true ) {\n}";
            string boundry = "\n---\n";
            string all = variables + boundry + whileandif + boundry + function + boundry + sub + boundry + clas + boundry + end;
            Input.Text = whileandif;

            Start();
        }

        public void Start()
        {
            //TODO: For and Foreach Loops & cases
            //TODO: Spaces on Ifs and functions

            //TODO: add names and stars when edited

            //TODO: over prepared
            //TODO: VB

            #region Setup

            List<string> cSharpWords = new List<string>
            { "if", "for", "foreach", "Char", "string", "int", "", "as", "", "", "void", "", "", "}", "=", "=","if","while","class","else","true","false","do"};

            List<string> pseudoCodeWords = new List<string>
            { "IF", "FOR", "FOREACH", "CHAR", "STRING", "INT", "DEFINE", "AS", "SUBROUTINE", "SUBROUTINE", "VOID", "DO", "THEN", "END","TO","TO","IF","WHILE","CLASS","ELSE","TRUE","FALSE","DO" }; // redo

            List<string> cFlatKeywords = new List<string>
            { "If", "For", "Foreach", "Char", "String", "Int", "Def", "As", "Function", "Sub", "Void", "", "", "End","ASG","VAL","If","While","Class","Else","True","False","Do"};

            arrangement cSharpSetup = new arrangement("{0} void {1} {2} {", "{0} {3} {1} {2} {", "{0} class {1} {", "{0} {1};", "{0} = {1};", "{0} {1} = {2};", "if ( {0} ) {", "while ( {0} ) {", "{0} {1} {2};", "{0} {1} {2} = {3};", "void {0} {1} {", "{2} {0} {1} {", "class {0}", "}");
            arrangement pseudoCodeSetup = new arrangement("{0} SUBROUTINE {1} {2}", "{0} {3} SUBROUTINE {1} {2}","{0} CLASS {1}","DEFINE {1} AS {0}","SET {0} TO {1}","DEFINE {1} AS {0} = {2}","IF {0} THEN","WHILE {0} DO","DEFINE {2} AS {0} {1}","DEFINE {2} AS {0} {1} = {3}", "SUBROUTINE {0} {1}", "{2} SUBROUTINE {0} {1}","CLASS {0}","END {0}");
            arrangement cFlatSetup = new arrangement("{0} Sub {1} {2} NRS", "{0} Function {1} {2} RT {3} FS", "{0} Class {1} CP", "Def {0} {1}", "{0} VAL {1}", "Def {0} {1} ASG {2}", "If {0} AGO", "While {0} BEG", "{0} Def {1} {2}","{0} Def {1} {2} ASG {3}","Sub {0} {1} NRS","Function {0} {1} RT {2} FS","Class {0} CP","End {0}");

            #endregion

            Cs = new Language("C#", ";", cSharpWords, cSharpSetup);
            PseudoCode = new Language("PseudoCode", "", pseudoCodeWords, pseudoCodeSetup);
            Cb = new Language("C flat", "", cFlatKeywords, cFlatSetup);

            languages.AddRange(new List<Language> { PseudoCode, Cs, Cb  });

            foreach (Language lang in languages)
            {
                fromComboBox.Items.Add(lang.name);
                toComboBox.Items.Add(lang.name);
            }

            // 0 and 1

            fromComboBox.SelectedIndex = 0;
            toComboBox.SelectedIndex = 1;
        }

        private Language Fetcher(string _name)
        {
            Language output = null;

            foreach(Language lang in languages)
            {
                if(lang.name == _name)
                {
                    output = lang;
                }
            }

            if(output == null)
            {
                output = null;
            }

            return output;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Language one = Fetcher(fromComboBox.SelectedItem.ToString());
            Language two = Fetcher(toComboBox.SelectedItem.ToString());

            if(one == two)
            {
                MessageBox.Show("Irrelivant entry of languages", "Request Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Control.ControlCollection inputControls = Control.Controls;

                // disable

                foreach(Control inputControl in inputControls)
                {
                    inputControl.Enabled = false;
                }

                Output.Text = Convert(one, two, Input.Text);
                MessageBox.Show("Conversion Complete between "+one.name+" and "+two.name, "Conversion Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // reanable

                foreach (Control inputControl in inputControls)
                {
                    inputControl.Enabled = true;
                }
            }
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            string Error = "";

            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            openFileDialog1.InitialDirectory = @"C:\";
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    string text = File.ReadAllText(file);
                    Input.Text = text;
                }
                catch (IOException)
                {

                }
            }
            else
            {
                if (result == DialogResult.Cancel)
                {
                    Error = "1 - You have not selected a file to import!";

                    MessageBox.Show("Error " + Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Converted "+toComboBox.SelectedItem.ToString()+" (*.txt)|.txt";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save As";

            DialogResult dr = saveFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, Output.Text);
            }
        }

        public class CustomProgressBar
        {
            int progression = 0;
            double visualProgression = 0;

            public double VisualValue
            {
                get
                {
                    return visualProgression;
                }
                set
                {
                    visualProgression = value;
                }
            }

            public int Value
            {
                get
                {
                    return progression;
                }
                set
                {
                    progression = value;

                    if(progression == 0)
                    {
                        visualProgression = 1;
                    }

                    ActiveForm.Invalidate();
                }
            }

            public Color foreColor;
            public Color backColor;
            public bool Visable = true;

            public Size Size = new Size(0,0);
            public Point Location = new Point(0,0);
        }

        private void Control_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            SolidBrush front = new SolidBrush(progressBar.foreColor);
            SolidBrush back = new SolidBrush(progressBar.backColor);

            int buffer = 0;

            g.FillRectangle(back, new Rectangle(progressBar.Location, progressBar.Size));

            Size progressRect = new Size(progressBar.Size.Width - buffer * 2, progressBar.Size.Height - buffer * 2);

            if (progressBar.Value > progressBar.VisualValue)
            {
                if (progressBar.Value != 100)
                {
                    progressBar.VisualValue = progressBar.VisualValue + 1;
                    progressRect.Width = int.Parse(((((progressRect.Width / 100) * (progressBar.VisualValue)))).ToString());
                }
            }
            else
            {
                progressBar.VisualValue = progressBar.Value;
                progressRect.Width = int.Parse(((((progressRect.Width / 100) * (progressBar.VisualValue)))).ToString());
            }

            g.FillRectangle(front, new Rectangle(new Point(progressBar.Location.X + buffer, progressBar.Location.Y + buffer), progressRect));
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            Control.Invalidate();
        }

        private void SwapButton_Click(object sender, EventArgs e)
        {
            Control.ControlCollection inputControls = Control.Controls;

            // disable

            foreach (Control inputControl in inputControls)
            {
                inputControl.Enabled = false;
            }

            int fromOption = fromComboBox.SelectedIndex;

            fromComboBox.SelectedItem = toComboBox.SelectedItem;
            toComboBox.SelectedIndex = fromOption;

            string inputText = Input.Text;

            Input.Text = Output.Text;
            Output.Text = inputText;

            // reanable

            foreach (Control inputControl in inputControls)
            {
                inputControl.Enabled = true;
            }
        }
    }
}