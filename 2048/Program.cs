using System;
using System.Collections.Generic;
using System.Linq;

namespace _2048
{
    class DB
    {
        //2个list的DB
        public bool DoCreate { get; set; }//判断是否出现移动，如果无则不生成新的2
        public int Max { get; set; }
        static int Modenum = 2;
        private Dictionary<int, List<int>> DBHorizontal { get; set; }
        private Dictionary<int, List<int>> DBVertical { get; set; }
        public DB()
        {
            this.DoCreate = true;
            this.Max = 0;
            this.DBHorizontal = new Dictionary<int, List<int>>();
            this.DBVertical = new Dictionary<int, List<int>>();
            for (int i = 0; i < 4; i++)
            {
                this.DBHorizontal.Add(i, new List<int> { 0, 0, 0, 0 });
                this.DBVertical.Add(i, new List<int> { 0, 0, 0, 0 });
            }
        }
        private void IVforeach(Action<int, int> a)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int v = 0; v < 4; v++)
                {
                    a(i, v);
                }
            }
        }
        //同步DB
        public void Syn(Dictionary<int, List<int>> from, Dictionary<int, List<int>> to)
        {
            this.IVforeach((i, v) => { to[v][i] = from[i][v]; });
        }
        //创造新的2
        public void create(Dictionary<int, List<int>> Now)
        {
            int count = 0;//检查全场0的个数
            this.IVforeach((i, v) =>
            {
                if (Now[i][v] > this.Max)
                {
                    this.Max = Now[i][v];//检测全场最大值
                }
                if (Now[i][v] == 0)
                {
                    count++;
                }
            });

            int Ran = new Random().Next(0, count - 1);//创造随机数，决定2出现在那个0上
            int _count = 0;//指针，筛选0所在的格子
            this.IVforeach((i, v) =>
            {
                if (Now[i][v] == 0)
                {
                    if (_count == Ran)
                    {
                        Now[i][v] = Modenum;
                        _count++;
                    }
                    else { _count++; }
                }
            }
            );
        }
        public void Move(string direction)
        {
            switch (direction)
            {
                case "a"://左 
                    this.DBHorizontal = this.MoveCalculator(DBHorizontal, true);
                    this.AfterMove(true);
                    break;
                case "d"://右
                    this.DBHorizontal = this.MoveCalculator(DBHorizontal, false);
                    this.AfterMove(true);
                    break;
                case "w"://上
                    this.DBVertical = this.MoveCalculator(DBVertical, true);
                    this.AfterMove(false);
                    break;
                case "s"://下
                    this.DBVertical = this.MoveCalculator(DBVertical, false);
                    this.AfterMove(false);
                    break;
                case " ":
                    {
                        throw new Exception("难道你也是演员！  （你没有输入任何东西）");
                    }
                default:
                    {
                        throw new Exception("睁眼按瞎键！  （你输入了莫名其妙的字符）");
                    }
            }
        }
        public Dictionary<int, List<int>> MoveCalculator(Dictionary<int, List<int>> db, bool reverse)
        {
            int _count = 0;
            for (int v = 0; v < 4; v++)
            {
                if (reverse)
                {//反向，为了达到右向左计算
                    db[v].Reverse();
                }
                var _list = new List<int>(db[v]);

                for (int B = 0; B < db[v].Count; B++)
                {//移动格子到角落
                    if (db[v][B] == 0)
                    {
                        db[v].RemoveAt(B);//删除0且把0放在开头
                        db[v].Insert(0, 0);
                    }
                }
                for (int i = 3; i > 0; i--)
                {
                    if (db[v][i] == db[v][i - 1] && db[v][i] != 0)
                    {
                        db[v][i - 1] = 0;
                        db[v][i] += db[v][i];
                        i--;//如果下一个数值和自己一样，则自己变0别人翻倍
                    }
                }
                for (int B = 0; B < db[v].Count; B++)
                {//移动格子到角落
                    if (db[v][B] == 0)
                    {
                        db[v].RemoveAt(B);//删除0且把0放在开头
                        db[v].Insert(0, 0);
                        _count++;
                    }
                }

                if (!_list.SequenceEqual(db[v]))
                {
                    this.DoCreate = true;
                }
                if (reverse)
                {
                    db[v].Reverse();//翻回来
                }
            }
            if (_count == 0)
            {
                this.Max = 10000;//设置最大值为100
            }
            return db;
        }
        public void AfterMove(bool a)
        {//true H=>V; False V => H
            if (this.DoCreate)
            {
                if (a)
                {
                    this.create(this.DBHorizontal);
                    this.Syn(this.DBHorizontal, this.DBVertical);
                }
                else
                {
                    this.create(this.DBVertical);
                    this.Syn(this.DBVertical, this.DBHorizontal);
                }
                this.DoCreate = false;
            }
        }
        public void Read()
        {
            Console.WriteLine("-----------2048-----------");
            for (int i = 0; i < 4; i++)
            {
                foreach (var M in DBHorizontal[i])
                {
                    string Q = M.ToString();
                    if (Q == "0") { Q = "-"; }
                    Console.Write(Q + "\t");
                }
                Console.WriteLine("\n\n");
            }
            Console.WriteLine("虚拟手柄：A => 左，D => 右，W => 上，S => 下");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            DB _Q2048 = new DB();
            Console.WriteLine("Hello 2048!\n由于控制台功能限制\n你所输入的内容中只有最后一个字符生效");
            _Q2048.AfterMove(true);
            _Q2048.Read();
            while (_Q2048.Max < 2048)
            {
                try
                {
                    string i = " " + Console.ReadLine().ToLowerInvariant();
                    _Q2048.Move(i[i.Length - 1].ToString());
                    _Q2048.Read();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            if (_Q2048.Max == 10000)
            {
                Console.WriteLine("你凉了，玩你MA呢! （你输了）\n按任意键退出");
            }
            else
            {
                Console.WriteLine("你已经赢我太多！ （你赢了）\n按任意键退出");
            }
            Console.Read();
        }
    }
}
