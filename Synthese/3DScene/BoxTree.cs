using System;
using System.Collections.Generic;
using System.Text;

namespace TPSynthese
{
    class BoxTree
    {
        public float maxBoxSize;
        public BoundingBox topBox;

        public abstract class BoundingBox
        {
            public Box box;
        }

        public class NodeBoundingBox : BoundingBox
        {
            public List<BoundingBox> underBoxes;
        }

        public class TipBoundingBox : BoundingBox
        {
            public List<iSceneObject> content;
        }

        public BoxTree () { }
        
        public BoxTree (float maxSize, List<iSceneObject> objects)
        {
            maxBoxSize = maxSize;
            List<BoundingBox> individualBoxes = BoundEachObject(objects);
            topBox = BuildTreeFrom (UnionBoundingBox(individualBoxes));            
        }

        private BoundingBox BuildTreeFrom (BoundingBox top)
        {
            if (top is TipBoundingBox) return top;

            NodeBoundingBox topNode = top as NodeBoundingBox;

            if (topNode.underBoxes.Count == 0)
            {
                return null;
            }

            if (topNode.underBoxes.Count == 1)
            {
                return topNode.underBoxes[0] as TipBoundingBox;
            }

            NodeBoundingBox newTop = new NodeBoundingBox();
            newTop.box = top.box;
            newTop.underBoxes = new List<BoundingBox>();

            NodeBoundingBox underBox1 = new NodeBoundingBox();
            List<BoundingBox> firstHalf = new List<BoundingBox>(topNode.underBoxes);
            firstHalf.RemoveRange(0, topNode.underBoxes.Count / 2);

            newTop.underBoxes.Add(BuildTreeFrom (UnionBoundingBox (firstHalf)));

            NodeBoundingBox underBox2 = new NodeBoundingBox();
            List<BoundingBox> secondHalf = new List<BoundingBox>(topNode.underBoxes);
            secondHalf.RemoveRange(topNode.underBoxes.Count / 2, topNode.underBoxes.Count - firstHalf.Count);

            newTop.underBoxes.Add(BuildTreeFrom (UnionBoundingBox (secondHalf)));

            return newTop;
        }

        private List<BoundingBox> BoundEachObject(List<iSceneObject> objects, bool ignoreBigObjects = true)
        {
            List<BoundingBox> bBoxes = new List<BoundingBox>();

            foreach (iSceneObject so in objects)
            {
                if (so.Size <= maxBoxSize)
                {
                    TipBoundingBox bb = new TipBoundingBox();
                    bb.box = so.BoundingBox;
                    bb.content = new List<iSceneObject>();
                    bb.content.Add(so);

                    bBoxes.Add(bb);
                }
            }

            return bBoxes;
        }

        private NodeBoundingBox UnionBoundingBox (List<BoundingBox> boxes)
        {
            if (boxes.Count == 0) return null;

            NodeBoundingBox bBox = new NodeBoundingBox();
            bBox.box = new Box(double.MaxValue * new Vector3Double(1, 1, 1), double.MinValue * new Vector3Double(1, 1, 1));

            foreach (BoundingBox bb in boxes)
            {
                bBox.box.minPoint = new Vector3Double
                    (
                    Math.Min(bBox.box.minPoint.X, bb.box.minPoint.X),
                    Math.Min(bBox.box.minPoint.Y, bb.box.minPoint.Y),
                    Math.Min(bBox.box.minPoint.Z, bb.box.minPoint.Z)
                    );

                bBox.box.maxPoint = new Vector3Double
                    (
                    Math.Max(bBox.box.maxPoint.X, bb.box.maxPoint.X),
                    Math.Max(bBox.box.maxPoint.Y, bb.box.maxPoint.Y),
                    Math.Max(bBox.box.maxPoint.Z, bb.box.maxPoint.Z)
                    );
            }

            bBox.underBoxes = boxes;

            return bBox;
        }
    }
}
