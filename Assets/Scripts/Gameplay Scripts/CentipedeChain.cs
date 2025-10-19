using System.Collections.Generic;
using UnityEngine;

public class CentipedeChain
{
    public readonly List<SegmentController> chainSegments = new();

    private const int MAX_PATH_SIZE = 64;
    private readonly Vector2[] positionHistory = new Vector2[MAX_PATH_SIZE];
    private int headIndex = 0;

    public Vector2 horizontalDirection = Vector2.right;
    public Vector2 verticalDirection = Vector2.down;
    public bool isDiving = false;
    public Vector2 HeadPosition => positionHistory[headIndex];


    public CentipedeChain(List<SegmentController> initialSegments)
    {
        chainSegments.AddRange(initialSegments);

        for (int i = 0; i < chainSegments.Count; i++)
        {
            positionHistory[i] = chainSegments[i].transform.position;
        }
    }

    private CentipedeChain(List<SegmentController> newSplitSegments, Vector2[] sourcePath, int sourceHeadIndex, int splitStartIndex, int segmentCount)
    {
        chainSegments.AddRange(newSplitSegments);

        for (int i = 0; i < segmentCount; i++)
        {
            int readIndex = (sourceHeadIndex + splitStartIndex + i) % MAX_PATH_SIZE;  
            positionHistory[i] = sourcePath[readIndex];
        }
    }

    public void AdvancePath(Vector2 nextHeadPosition)
    {
        headIndex = (headIndex - 1 + MAX_PATH_SIZE) % MAX_PATH_SIZE; // push a new head position into the ring buffer by moving the headIndex backward
        positionHistory[headIndex] = nextHeadPosition;
    }

    public void ApplyPathToSegments() // assign each segment a target position along the stored path
    {
        for (int i = 0; i < chainSegments.Count; i++)
        {
            int pathIndex = (headIndex + i) % MAX_PATH_SIZE;
            chainSegments[i].SetTargetPosition(positionHistory[pathIndex]);
        }
    }

    public void RefreshAppearance(GameConfig gameConfig, bool isEntering) // update sprites and orientation for head/body/tail
    {
        for (int i = 0; i < chainSegments.Count; i++)
        {
            Sprite segmentSprite = gameConfig.bodySprite;
            bool isVertical = isEntering || isDiving;

            if (i == 0)
            {
                segmentSprite = gameConfig.headSprite;
            }
            else if (i == chainSegments.Count - 1)
            {
                segmentSprite = gameConfig.tailSprite;
            }

            chainSegments[i].SetAppearance(segmentSprite, horizontalDirection.x < 0f);
            chainSegments[i].SetDiveRotation(isVertical, horizontalDirection.x);
        }
    }

    public CentipedeChain Split(int startIndex)
    {
        CentipedeChain newSplitChain = null;

        if (startIndex > 0 && startIndex < chainSegments.Count)
        {
            int splitCount = chainSegments.Count - startIndex;
            List<SegmentController> newChainSegments = chainSegments.GetRange(startIndex, splitCount);

            newSplitChain = new CentipedeChain(newChainSegments, positionHistory, headIndex, startIndex, splitCount)
            {
                horizontalDirection = -horizontalDirection,
                verticalDirection = verticalDirection,
                isDiving = isDiving
            };

            chainSegments.RemoveRange(startIndex, splitCount);
        }

        return newSplitChain;
    }
}