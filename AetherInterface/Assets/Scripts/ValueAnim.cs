using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public delegate float Ease(float t);

public static class Easing {

    // Easing functions
    // Cheat sheet: http://easings.net/
    public static float linear(float t) {
        return t;
    }

    public static float easeInQuad(float t) {
        return t * t;
    }

    public static float easeInCubic(float t) {
        return t * t * t;
    }

    public static float easeInQuart(float t) {
        return t * t * t * t;
    }

    public static float easeInElastic(float t) {
        return (0.04f - 0.04f / t) * Mathf.Sin(25.0f * t) + 1;
    }
}

public interface IAnimable<T> {
    T Add(T a, T b);
    T Sub(T a, T b);
    T Scale(T a, float b);
}

public struct FloatAnimable : IAnimable<float> {
    public float Add(float a, float b) { return a + b; }

    public float Sub(float a, float b) { return a - b; }

    public float Scale(float a, float b) { return a * b; }
}

public struct Vector2Animable : IAnimable<Vector2> {
    public Vector2 Add(Vector2 a, Vector2 b) { return a + b; }

    public Vector2 Sub(Vector2 a, Vector2 b) { return a - b; }

    public Vector2 Scale(Vector2 a, float b) { return a * b; }
}

public struct Vector3Animable : IAnimable<Vector3> {
    public Vector3 Add(Vector3 a, Vector3 b) { return a + b; }

    public Vector3 Sub(Vector3 a, Vector3 b) { return a - b; }

    public Vector3 Scale(Vector3 a, float b) { return a * b; }
}

// Animates a value based off an easing function
public class ValueAnim<T, C>
    where C : IAnimable<T>, new() {
    T start; // Value to start at
    T end; // Value to animate towards
    float duration; // Duration in seconds of the animation
    float timer; // Elapsed time of the animation in seconds
    Ease fn; // Selected easing function

    C animable; // Animable class for arithmetic

    public bool reverse;
    private bool playing;

    public ValueAnim(T _start, T _end, float _duration, Ease _fn) {
        // Set the given values
        start = _start;
        end = _end;
        duration = _duration;
        fn = _fn;


        // Set default values
        timer = 0.0f;
        reverse = false;
        playing = true;

        // Instance our arithmetic class
        animable = new C();
    }

    public T Update(float deltaTime) {
        if (timer <= 0.0f) {
            timer += reverse ? -deltaTime : deltaTime;
            if(reverse) playing = false;
            return start;
        } else if (timer < duration) {
            float t = timer / duration; // range: [0,1] represents amount of animation completed
            float scalar = fn(t);
            timer += reverse ? -deltaTime : deltaTime;

            T val = animable.Add(animable.Scale(animable.Sub(end, start), scalar), start);
            return val;
        } else {
            timer += reverse ? -deltaTime : deltaTime;
            if (!reverse) playing = false;
            return end;
        }

    }

    public void Reset() {
        timer = reverse ? duration : 0.0f;
        playing = true;
    }

    public bool isPlaying() {
        return playing;
    }
}