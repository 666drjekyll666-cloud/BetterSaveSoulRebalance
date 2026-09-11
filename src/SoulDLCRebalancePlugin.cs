using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SoulDLCRebalance
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class SoulDLCRebalancePlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "nikich.graveyardkeeper.souldlcrebalance";
        public const string PluginName = "Better Save Soul Rebalance";
        public const string PluginVersion = "1.1.0";

        private static ManualLogSource Log;
        private static object Balance;
        private static bool Patched;
        private static readonly HashSet<string> Logged = new HashSet<string>(StringComparer.Ordinal);

        private sealed class TechPrice
        {
            public string Id; public float R, G, B, V, GP, NewB, NewGP;
        }
        private sealed class Need
        {
            public string Id; public int N;
            public Need(string id, int n) { Id = id; N = n; }
        }
        private sealed class GraveCraft
        {
            public string Id; public Need[] Stock, Desired; public int StockRemote, Local;
        }
        private sealed class UiTemp
        {
            public IList Needs; public object Pseudo;
        }

        private static Need N(string id, int n) { return new Need(id, n); }

        private static readonly TechPrice[] Techs =
        {
            new TechPrice{Id="soul_stone_fences",R=50,G=50,B=0,V=0,GP=0,NewB=150,NewGP=30},
            new TechPrice{Id="soul_buildings",R=20,G=15,B=0,V=0,GP=0,NewB=0,NewGP=40},
            new TechPrice{Id="soul_marble_fences",R=100,G=75,B=0,V=0,GP=0,NewB=200,NewGP=40},
            new TechPrice{Id="soul_sins_3",R=15,G=20,B=0,V=0,GP=15,NewB=0,NewGP=55},
            new TechPrice{Id="soul_stone_statues",R=80,G=150,B=0,V=0,GP=0,NewB=250,NewGP=45},
            new TechPrice{Id="soul_marble_statues",R=120,G=170,B=0,V=0,GP=0,NewB=300,NewGP=60},
            new TechPrice{Id="soul_buildings_2",R=30,G=20,B=0,V=0,GP=0,NewB=0,NewGP=55},
            new TechPrice{Id="soul_sins_4",R=25,G=25,B=0,V=0,GP=30,NewB=0,NewGP=70},
            new TechPrice{Id="soul_church_additions",R=25,G=50,B=0,V=0,GP=0,NewB=0,NewGP=20},
            new TechPrice{Id="soul_totem_tech",R=50,G=30,B=0,V=0,GP=0,NewB=0,NewGP=25},
            new TechPrice{Id="soul_writing_additions",R=40,G=35,B=10,V=0,GP=0,NewB=10,NewGP=35},
            new TechPrice{Id="soul_garden_additions",R=30,G=50,B=0,V=0,GP=0,NewB=0,NewGP=35}
        };

        private static readonly GraveCraft[] Graves =
        {
            new GraveCraft{Id="grave_bot_stn_6",StockRemote=5,Local=5,Stock=new[]{N("stone_plate_1",1),N("stone_plate_2",1)},Desired=new[]{N("stone_plate_2",1),N("stone_plate_3",1)}},
            new GraveCraft{Id="grave_bot_stn_7",StockRemote=5,Local=7,Stock=new[]{N("stone_plate_1",2),N("stone_plate_2",1),N("detail_3",2)},Desired=new[]{N("stone_plate_2",2),N("stone_plate_3",1),N("detail_3",2)}},
            new GraveCraft{Id="grave_bot_stn_8",StockRemote=5,Local=10,Stock=new[]{N("stone_plate_1",2),N("stone_plate_2",1),N("jewelry_detail_gold",1)},Desired=new[]{N("stone_plate_2",2),N("stone_plate_3",1),N("jewelry_detail_gold",1)}},
            new GraveCraft{Id="grave_bot_mrb_6",StockRemote=5,Local=10,Stock=new[]{N("marble_plate_1",1),N("marble_plate_2",1)},Desired=new[]{N("marble_plate_2",1),N("marble_plate_3:1",1)}},
            new GraveCraft{Id="grave_bot_mrb_7",StockRemote=5,Local=12,Stock=new[]{N("marble_plate_1",2),N("marble_plate_2",1),N("detail_3",2)},Desired=new[]{N("marble_plate_2",2),N("marble_plate_3:2",1),N("detail_3",2)}},
            new GraveCraft{Id="grave_bot_mrb_8",StockRemote=5,Local=15,Stock=new[]{N("marble_plate_1",2),N("marble_plate_2",1),N("jewelry_detail_gold",1)},Desired=new[]{N("marble_plate_2",2),N("marble_plate_3:3",1),N("jewelry_detail_gold",1)}},
            new GraveCraft{Id="grave_top_sculpt_stn_4",StockRemote=10,Local=12,Stock=new[]{N("stone_plate_2",1),N("stone_plate_3",1),N("detail_2",2)},Desired=new[]{N("stone_plate_2",2),N("stone_plate_3",1),N("detail_2",2)}},
            new GraveCraft{Id="grave_top_sculpt_stn_5",StockRemote=10,Local=15,Stock=new[]{N("stone_plate_2",2),N("stone_plate_3",2),N("detail_2",2)},Desired=new[]{N("stone_plate_2",2),N("stone_plate_3",2),N("detail_2",2),N("sin_shard",1)}},
            new GraveCraft{Id="grave_top_sculpt_mrb_4",StockRemote=15,Local=15,Stock=new[]{N("marble_plate_2",3),N("marble_plate_3:3",2),N("detail_2",2)},Desired=new[]{N("marble_plate_2",3),N("marble_plate_3:3",2),N("detail_2",2),N("sin_shard",1)}},
            new GraveCraft{Id="grave_top_sculpt_mrb_5",StockRemote=15,Local=15,Stock=new[]{N("marble_plate_2",3),N("marble_plate_3:3",3),N("detail_2",2)},Desired=new[]{N("marble_plate_2",3),N("marble_plate_3:3",3),N("detail_2",2),N("sin_shard",2)}}
        };

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo(PluginName + " " + PluginVersion + " loading.");
            StartCoroutine(InitWhenReady());
        }

        private IEnumerator InitWhenReady()
        {
            while (!R.BindGameAssembly()) yield return null;
            while (Balance == null)
            {
                Balance = R.GetStatic(R.GameType("GameBalance"), "me");
                if (Balance == null) yield return null;
            }
            if (!R.SoulsDlcAvailable()) { Log.LogWarning("Better Save Soul unavailable; mod disabled."); yield break; }
            try
            {
                InstallPatches();
                ApplyAll();
                Log.LogInfo(PluginName + " " + PluginVersion + " ready.");
            }
            catch (Exception ex) { Log.LogError("Initialization failed: " + ex); }
        }

        private static void InstallPatches()
        {
            if (Patched) return;
            Type owner = typeof(SoulDLCRebalancePlugin);
            R.Patch(PluginGuid + ".reload", owner, R.Method(R.GameType("TechDefinition"), "LinkTechs", true, 0), null, "LinkTechsPostfix", null);
            R.Patch(PluginGuid + ".can", owner, R.Method(R.GameType("BaseCraftGUI"), "CanCraft", false, 4), null, "CanCraftPostfix", null);
            R.Patch(PluginGuid + ".start", owner, R.Method(R.GameType("CraftComponent"), "CraftReally", false, 9), "CraftReallyPrefix", null, null);
            R.Patch(PluginGuid + ".finish", owner, R.Method(R.GameType("CraftComponent"), "FinishCurrentCraft", false, 0), "FinishPrefix", "FinishPostfix", null);
            R.Patch(PluginGuid + ".ui", owner, R.Method(R.GameType("CraftItemGUI"), "Redraw", false, 0), "UiPrefix", null, "UiFinalizer");
            Patched = true;
        }

        private static void LinkTechsPostfix()
        {
            try { Balance = R.GetStatic(R.GameType("GameBalance"), "me") ?? Balance; ApplyAll(); }
            catch (Exception ex) { Warn("reload", "Reapply failed: " + ex.Message); }
        }

        private static void ApplyAll() { ApplyTechs(); ApplyGraves(); }

        private static void ApplyTechs()
        {
            IList list = R.Get(Balance, "techs_data") as IList;
            if (list == null) { Warn("tech-list", "techs_data unavailable; no tech prices changed."); return; }
            foreach (TechPrice c in Techs)
            {
                object tech = Find(list, c.Id), price = R.Get(tech, "price");
                if (tech == null || price == null) { Warn("tech:" + c.Id, "Technology unavailable; left unchanged: " + c.Id); continue; }
                float r=R.GameResGet(price,"r"), g=R.GameResGet(price,"g"), b=R.GameResGet(price,"b"), v=R.GameResGet(price,"v"), gp=R.GameResGet(price,"gratitude_points");
                bool desired = R.Eq(r,c.R)&&R.Eq(g,c.G)&&R.Eq(b,c.NewB)&&R.Eq(v,c.V)&&R.Eq(gp,c.NewGP);
                if (desired) continue;
                bool stock = R.Eq(r,c.R)&&R.Eq(g,c.G)&&R.Eq(b,c.B)&&R.Eq(v,c.V)&&R.Eq(gp,c.GP);
                if (!stock) { Warn("tech-base:"+c.Id,"Technology baseline mismatch; left unchanged: "+c.Id+" current="+Price(r,g,b,v,gp)); continue; }
                object copy = R.CloneGameRes(price); R.GameResSet(copy,"b",c.NewB); R.GameResSet(copy,"gratitude_points",c.NewGP); R.Set(tech,"price",copy,true);
                InfoOnce("tech:"+c.Id,"Tech price patched: "+c.Id+" -> "+Price(c.R,c.G,c.NewB,c.V,c.NewGP));
            }
        }

        private static void ApplyGraves()
        {
            IList list = R.Get(Balance, "craft_data") as IList;
            if (list == null) { Warn("craft-list", "craft_data unavailable; no grave recipes changed."); return; }
            foreach (GraveCraft c in Graves)
            {
                object craft = Find(list,c.Id); IList needs = R.Get(craft,"needs") as IList; object expr = R.Get(craft,"gratitude_points_craft_cost");
                if (craft==null || needs==null || expr==null) { Warn("craft:"+c.Id,"Craft data unavailable; left unchanged: "+c.Id); continue; }
                int remoteNew=c.StockRemote+c.Local; float remote=R.SmartFloat(expr);
                bool remoteStock=R.Eq(remote,c.StockRemote), remoteDesired=R.Eq(remote,remoteNew);
                bool recipeStock=Same(needs,c.Stock), recipeDesired=Same(needs,c.Desired);
                if ((!remoteStock&&!remoteDesired)||(!recipeStock&&!recipeDesired)) { Warn("craft-base:"+c.Id,"Craft baseline mismatch; left unchanged: "+c.Id+" needs="+Needs(needs)+" remoteGP="+remote.ToString("0.###",CultureInfo.InvariantCulture)); continue; }
                if (remoteDesired&&recipeDesired) continue;
                object oldNeeds=needs, oldExpr=expr;
                IList newNeeds=recipeDesired?needs:MakeNeeds(needs.GetType(),c.Desired);
                object newExpr=remoteDesired?expr:R.CloneSmartConstant(expr,remoteNew);
                try { if(!remoteDesired)R.Set(craft,"gratitude_points_craft_cost",newExpr,true); if(!recipeDesired)R.Set(craft,"needs",newNeeds,true); }
                catch { try{R.Set(craft,"gratitude_points_craft_cost",oldExpr,true);}catch{} try{R.Set(craft,"needs",oldNeeds,true);}catch{} throw; }
                InfoOnce("craft:"+c.Id,"Grave craft patched: "+c.Id+" needs="+Spec(c.Desired)+" localGP="+c.Local+" remoteGP="+remoteNew);
            }
        }

        private static void CanCraftPostfix(object[] __args, ref bool __result)
        {
            try { if(!__result||__args==null||__args.Length<3||R.GlobalCraftActive())return; int cost=Local(__args[0]); if(cost>0&&R.Gratitude()+.0001f<cost*Math.Max(1,Convert.ToInt32(__args[2],CultureInfo.InvariantCulture)))__result=false; }
            catch(Exception ex){Warn("can","Local GP eligibility check failed safe: "+ex.Message);__result=false;}
        }

        private static bool CraftReallyPrefix(object[] __args, ref bool __result)
        {
            try { if(__args==null||__args.Length<7)return true; int cost=Local(__args[0]); if(cost<=0||Convert.ToBoolean(__args[6],CultureInfo.InvariantCulture))return true; int amount=Math.Max(1,Convert.ToInt32(__args[5],CultureInfo.InvariantCulture)); if(R.Gratitude()+.0001f>=cost*amount)return true; __result=false; return false; }
            catch(Exception ex){Warn("start","Manual local GP start check failed safe: "+ex.Message);__result=false;return false;}
        }

        private static bool FinishPrefix(object __instance, ref int __state)
        {
            __state=0;
            try { object craft=R.Get(__instance,"current_craft"); int cost=Local(craft); if(cost<=0||R.Bool(__instance,"is_gratitude_points_spent_for_craft"))return true; if(R.Gratitude()+.0001f<cost){Warn("finish:"+R.Id(craft),"Craft completion held: insufficient local Soul Gratitude for "+R.Id(craft));return false;} __state=cost; return true; }
            catch(Exception ex){Warn("finish","Local GP finish check failed safe: "+ex.Message);return false;}
        }

        private static void FinishPostfix(int __state)
        {
            if(__state<=0)return;
            try { R.SetGratitude(Math.Max(0f,R.Gratitude()-__state)); }
            catch(Exception ex){Warn("charge","Completed-craft local GP charge failed: "+ex.Message);}
        }

        private static void UiPrefix(object __instance, ref UiTemp __state)
        {
            __state=null;
            try { if(R.GlobalCraftActive())return; object craft=R.Get(__instance,"current_craft")??R.Get(__instance,"craft_definition"); int cost=Local(craft); IList needs=R.Get(craft,"needs") as IList; if(cost<=0||needs==null)return; object pseudo=R.Item("gratitude_as_item",cost); needs.Add(pseudo); __state=new UiTemp{Needs=needs,Pseudo=pseudo}; }
            catch(Exception ex){Warn("ui","Local GP recipe display unavailable: "+ex.Message);}
        }

        private static Exception UiFinalizer(Exception __exception, UiTemp __state)
        {
            try { if(__state!=null&&__state.Needs!=null&&__state.Pseudo!=null)__state.Needs.Remove(__state.Pseudo); }
            catch(Exception ex){Warn("ui-clean","Temporary local GP UI cleanup failed: "+ex.Message);}
            return __exception;
        }

        private static int Local(object craft) { int n; return craft!=null&&LocalCosts.TryGetValue(R.Id(craft),out n)?n:0; }
        private static readonly Dictionary<string,int> LocalCosts = Graves.ToDictionary(x=>x.Id,x=>x.Local,StringComparer.Ordinal);
        private static object Find(IList list,string id){if(list==null)return null;for(int i=0;i<list.Count;i++)if(string.Equals(R.Id(list[i]),id,StringComparison.Ordinal))return list[i];return null;}
        private static bool Same(IList list,Need[] spec){if(list==null||list.Count!=spec.Length)return false;for(int i=0;i<spec.Length;i++){object v=R.Get(list[i],"value");if(!string.Equals(R.Id(list[i]),spec[i].Id,StringComparison.Ordinal)||v==null||Convert.ToInt32(v,CultureInfo.InvariantCulture)!=spec[i].N)return false;}return true;}
        private static IList MakeNeeds(Type type,Need[] spec){object[] a=new object[spec.Length];for(int i=0;i<a.Length;i++)a[i]=R.Item(spec[i].Id,spec[i].N);return R.NewList(type,a);}
        private static string Spec(Need[] s){return string.Join(",",s.Select(x=>x.Id+":"+x.N).ToArray());}
        private static string Needs(IList s){if(s==null)return "[null]";var p=new List<string>();for(int i=0;i<s.Count;i++)p.Add((R.Id(s[i])??"?")+":"+Convert.ToString(R.Get(s[i],"value"),CultureInfo.InvariantCulture));return "["+string.Join(",",p.ToArray())+"]";}
        private static string Price(float r,float g,float b,float v,float gp){return string.Format(CultureInfo.InvariantCulture,"{0:0}/{1:0}/{2:0}/{3:0}/{4:0}",r,g,b,v,gp);}
        private static void Warn(string k,string s){if(Logged.Add("W:"+k)&&Log!=null)Log.LogWarning(s);}
        private static void InfoOnce(string k,string s){if(Logged.Add("I:"+k)&&Log!=null)Log.LogInfo(s);}
    }
}
