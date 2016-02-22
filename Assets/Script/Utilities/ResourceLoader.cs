using UnityEngine;
using System.Collections;
using System.Linq;

public static class ResourceLoader {

    const string iconPath = "Spells Icon/64x64/Colored/";
    public static class Buff {
        public static Texture2D GetBuffIcon(PrimaryAttributeName name) {
            switch (name)
            {
                case PrimaryAttributeName.Power:
                    return Resources.Load(iconPath + "icon_64x64_23") as Texture2D;
                case PrimaryAttributeName.Agility:
                    return Resources.Load(iconPath + "icon_64x64_54") as Texture2D;
                case PrimaryAttributeName.Wisdom:
                    return Resources.Load(iconPath + "icon_64x64_21") as Texture2D;                    
                case PrimaryAttributeName.Constitution:
                    return Resources.Load(iconPath + "icon_64x64_56") as Texture2D;
                case PrimaryAttributeName.Luck:
                    return Resources.Load(iconPath + "icon_64x64_87") as Texture2D;
                default:
                    return Resources.Load(iconPath + "icon_64x64_151") as Texture2D;
            }
        }
    }
    public static class Skill
    {
        const string iconPath = "SkillIcon/";
        public static Texture2D GetIcon(SkillName name)
        {
            return Resources.Load(iconPath + name.ToString()) as Texture2D;            
        }
        public static GameObject GetSkillPrefab(SkillName name) {
            return Resources.Load("SkillPrefabs/"+ name.ToString()) as GameObject;
        }
        public static GameObject GetSkillCastingPrefab()
        {
            return Resources.Load("SkillPrefabs/Casting") as GameObject;
        }
    }
    public static class UIPrefabs {
        public static GameObject GetHurtText() {
            return Resources.Load("OnCharacterText") as GameObject;
        }
        public static GameObject BuffIcon()
        {
            return Resources.Load("BuffIcon") as GameObject;
        }
        public static GameObject GetBuildBlood() {
            return  Resources.Load("GUI/BuildBlood") as GameObject;
        }
        public static GameObject GetStayPrefabd(NPCJobs job) {
            return  Resources.Load("GUI/StayPref/"+job.ToString()) as GameObject;
        }
    }
    public static class ColorMaterial {
        public static Material GetNPCColor() {
            return Resources.Load("NPCTypeColor") as Material;
        }
    }
    public static class NPCTypeObject {
        public static Mesh GetCastleCylinder(NPCType type) {
            Mesh[] meshes = Resources.LoadAll<Mesh>("CastleBannerMeshFilter/BannerMeshes");
            if (type == NPCType.Friend) 
                return meshes.First(m=> m.name =="Cylinder");            
            if (type == NPCType.Enemy)
                return meshes.First(m => m.name == "Cylinder_002");
            else
                return meshes.First(m => m.name == "Cylinder_002");
        }
        public static Mesh GetCastlePlane(NPCType type)
        {
            Mesh[] meshes = Resources.LoadAll<Mesh>("CastleBannerMeshFilter/BannerMeshes");
            if (type == NPCType.Friend)
                return meshes.First(m => m.name == "Plane_003");
            if (type == NPCType.Enemy)
                return meshes.First(m => m.name == "Plane");
            else
                return meshes.First(m => m.name == "Plane");
        }
        public static Mesh GetCastleMainFlag(NPCType type)
        {
            Mesh[] meshes = Resources.LoadAll<Mesh>("CastleBannerMeshFilter/BannerMeshes");
            if (type == NPCType.Friend)
                return meshes.First(m => m.name == "Cylinder_001");
            if (type == NPCType.Enemy)
                return meshes.First(m => m.name == "Cylinder_005");
            else
                return meshes.First(m => m.name == "Cylinder_005");
        }
    }
    public static class NPC {
        public static GameObject GetNPCPrefab(NPCJobs job) {
            if(job == NPCJobs.Monster)
                return Resources.Load("CharacterPrefabs/Monster/Skeleton") as GameObject;
            return Resources.Load("CharacterPrefabs/" + job.ToString()) as GameObject;
        }
        public static string GetName(NPCJobs job) {
            switch (job)
            {
                case NPCJobs.Warrior:
                    break;
                case NPCJobs.Archer:
                    break;
                case NPCJobs.Mage:
                    break;
                case NPCJobs.Monster:
                    break;
                default:
                    break;
            }
            return job.ToString()+Random.Range(0,1000).ToString();
        }

        public static Texture2D GetFaceicon(NPCJobs job)
        {
            return Resources.Load<Texture2D>(job.ToString());
        }
    }

    public static class Utilities {
        public static Mesh GetCirclePlane() {
            return Resources.Load<Mesh>("CirclePlane");
        }
        public static RenderTexture GetEargleEye() {
            return Resources.Load<RenderTexture>("EARGLEEYE");
        }
    }
}
