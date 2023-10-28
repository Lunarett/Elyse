using UnityEngine;

namespace Pulsar.Utils
{
    public static class Utils
    {
        public static void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (obj == null) return;
            
            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        public static Transform FindChildByName(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name) return child;

                Transform result = FindChildByName(child, name);
                if (result != null) return result;
            }

            return null;
        }

        public static void DestroyAllChildren(GameObject obj)
        {
            foreach (Transform child in obj.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public static void ResetTransform(GameObject obj)
        {
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }

        public static void CopyTransform(GameObject source, GameObject destination)
        {
            destination.transform.position = source.transform.position;
            destination.transform.rotation = source.transform.rotation;
            destination.transform.localScale = source.transform.localScale;
        }

        public static bool IsVisibleFrom(GameObject obj, Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, obj.GetComponent<Collider>().bounds);
        }
    }
}