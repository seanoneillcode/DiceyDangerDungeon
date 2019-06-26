using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lovely
{
    public class LevelGenerator : MonoBehaviour
    {
        private Game game;

        public GameObject nodePrefab;
        public GameObject linkPrefab;
        public GameObject goalPrefab;
        public GameObject ghostPrefab;
        public GameObject dwarfPrefab;
        public GameObject healthPrefab;
        public GameObject potionPrefab;
        public GameObject poisonPrefab;
        public GameObject swordPrefab;
        public GameObject armourPrefab;
        public GameObject playerPrefab;
        public GameObject enemyPrefab;
        public GameObject goblinPrefab;
        public GameObject orcPrefab;
        public GameObject shamanPrefab;
        public GameObject wargPrefab;
        public GameObject trollPrefab;
        public GameObject golemPrefab;
        public GameObject goldPrefab;
        public GameObject prisonerPrefab;
        public GameObject portalPrefab;
        public GameObject trapPrefab;
        public GameObject permHealthPrefab;
        public GameObject permRollIncPrefab;
        public GameObject permShieldIncPrefab;
        public Transform map;


        public const int SIZE = 6;
        private LayoutGenerator layoutGenerator;

        // Start is called before the first frame update
        void Start()
        {
            game = FindObjectOfType<Game>();
            layoutGenerator = GetComponent<LayoutGenerator>();
            GenerateLevel();
        }

        public void GenerateLevel()
        {
            Point startPoint = layoutGenerator.GenerateLayout();
            List<Point> points = layoutGenerator.GetAllPoints();

            foreach (Point point in points)
            {
                ConvertPoint(point);
            }
            foreach (PointLink pointLink in layoutGenerator.links)
            {
                CreateLink(pointLink.from, pointLink.to);
            }

            GameObject playerObj = Instantiate(playerPrefab, startPoint.pos, Quaternion.identity, map);
            Player player = playerObj.GetComponent<Player>();
            player.speed = 4f;
            game.player = player;
            game.selectedPlayer = player;
        }

        private void ConvertPoint(Point point)
        {
            GameObject nodeTypePrefab = GetPrefab(point.type);
            GameObject nodeObject = Instantiate(nodeTypePrefab, point.pos, Quaternion.identity, map);
            Node node = nodeObject.GetComponent<Node>();
            point.node = node;
            node.risk = point.risk;
            if (point.type == PointType.RISK)
            {
                GameObject prefab = enemyPrefab;
                switch (point.risk)
                {
                    case 1:
                        prefab = goblinPrefab;
                        break;
                    case 2:
                        prefab = wargPrefab;
                        break;
                    case 3:
                        prefab = orcPrefab;
                        break;
                    case 4:
                        prefab = shamanPrefab;
                        break;
                    case 5:
                        prefab = trollPrefab;
                        break;
                    case 6:
                        prefab = golemPrefab;
                        break;
                }
                GameObject enemyObject = Instantiate(prefab, point.pos, Quaternion.identity, map);
                Actor enemyActor = enemyObject.GetComponent<Actor>();
                node.actor = enemyActor;
                enemyActor.node = node;
            }
            Pickup pickup = nodeObject.GetComponent<Pickup>();
            if (pickup != null)
            {
                pickup.amount = point.amount;
            }
        }

        private void CreateLink(Point from, Point to)
        {
            if (from != null && to != null)
            {
                GameObject linkObj = Instantiate(linkPrefab, (from.pos + to.pos) / 2f, Quaternion.identity, map);
                if (to.pos.x != from.pos.x)
                {
                    linkObj.transform.Rotate(0, 90, 0);
                }
                Link link = linkObj.GetComponent<Link>();
                link.nodeA = from.node;
                link.nodeB = to.node;
                from.node.AddLink(link);
                to.node.AddLink(link);
            }
        }

        private GameObject GetPrefab(PointType type)
        {
            if (type == PointType.END)
            {
                return goalPrefab;
            }
            if (type == PointType.HEALTH)
            {
                return healthPrefab;
            }
            if (type == PointType.POTION)
            {
                return potionPrefab;
            }
            if (type == PointType.POISON)
            {
                return poisonPrefab;
            }
            if (type == PointType.GHOST)
            {
                return ghostPrefab;
            }
            if (type == PointType.FRIEND)
            {
                return dwarfPrefab;
            }
            if (type == PointType.SWORD)
            {
                return swordPrefab;
            }
            if (type == PointType.ARMOUR)
            {
                return armourPrefab;
            }
            if (type == PointType.GOLD)
            {
                return goldPrefab;
            }
            if (type == PointType.PRISONER)
            {
                return prisonerPrefab;
            }
            if (type == PointType.TELEPORT)
            {
                return portalPrefab;
            }
            if (type == PointType.TRAP)
            {
                return trapPrefab;
            }
            if (type == PointType.PERM_HEALTH_INC)
            {
                return permHealthPrefab;
            }
            if (type == PointType.PERM_ROLL_INC)
            {
                return permRollIncPrefab;
            }
            if (type == PointType.PERM_START_ARMOUR)
            {
                return permShieldIncPrefab;
            }
            return nodePrefab;
        }
    }
}