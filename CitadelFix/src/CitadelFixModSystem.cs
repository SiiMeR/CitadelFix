using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.Common;

namespace CitadelFix
{
    public class CitadelFixModSystem : ModSystem
    {

        internal static CitadelFixModSystem modSystem;
        public EnumAppSide Side => api.Side;
        private ICoreAPI api;

        private List<Type> clientFixes = new List<Type>();
        private List<Type> serverFixes = new List<Type>();
        private List<Type> universalFixes = new List<Type>();
        public override void StartPre(ICoreAPI api)
        {
            base.StartPre(api);
            modSystem = this;
            this.api = api;
            this.LoadFixes();
            this.ExecuteFixes(GetFixes(), ModRunPhase.Pre);
        }

        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            this.ExecuteFixes(GetFixes(), ModRunPhase.Start);
        }

        public override void AssetsFinalize(ICoreAPI api)
        {
            base.AssetsFinalize(api);
            this.ExecuteFixes(GetFixes(), ModRunPhase.AssetsFinalize);
        }

        public override void AssetsLoaded(ICoreAPI api)
        {
            base.AssetsLoaded(api);
            this.ExecuteFixes(GetFixes(), ModRunPhase.AssetsLoaded);
        }


        public override void Dispose()
        {
            //HarmonyPatches.Dispose();
            this.DisposeFixes(GetFixes());
            if(Side != EnumAppSide.Universal){
                this.DisposeFixes(universalFixes);
            }
            base.Dispose();
        }

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return true;
        }

        public List<Type> GetFixes(){
            switch(this.Side){
                case EnumAppSide.Server:
                    return serverFixes;
                case EnumAppSide.Client:
                    return clientFixes;
                default:
                    return universalFixes;
            }
        }


        public void LoadFixes()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<Type> fixes = assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(CitadelFixThing)) && !t.IsAbstract && !t.IsAbstract)
                .ToList();
            modSystem.api.Logger.Notification($"Found fixes: {fixes.Count}");
            foreach(var fix in fixes){
                var side = fix.GetProperty<EnumAppSide>("Side");
                switch(side){
                    case EnumAppSide.Client:
                        clientFixes.Add(fix);
                        break;
                    case EnumAppSide.Server:
                        serverFixes.Add(fix);
                        break;
                    default:
                        clientFixes.Add(fix);
                        serverFixes.Add(fix);
                        universalFixes.Add(fix);
                        break;
                }

            }
        }

        public void ExecuteFixes(List<Type> fixes, ModRunPhase runPhase)
        {
            var filteredFixes = fixes.Where(t => {
                var fixRunPhase = t.GetProperty<ModRunPhase>("RunPhase");
                return fixRunPhase == runPhase;
            }).ToList();

            foreach(var fix in filteredFixes){
                try{
                    var applyMethod = fix.GetMethod("Apply", BindingFlags.Public | BindingFlags.Static);
                    applyMethod.Invoke(null, new object[] {modSystem, api});
                    api.Logger.Notification($"Executed fix: {fix.Name}");
                }catch(Exception err){
                    api.Logger.Error($"Error executing fix: {fix.Name} : {err}");
                }
            }
        }

        public void DisposeFixes(List<Type> fixes){
            foreach(var fix in fixes){
                try{
                    var disposeMethod = fix.GetMethod("Dispose", BindingFlags.Public | BindingFlags.Static);
                    disposeMethod.Invoke(null, new object[] {modSystem, api});
                }catch(Exception err){
                    api.Logger.Error($"Error disposing fix: {fix.Name} : {err}");
                }
            }
        }

    }
}
