 using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

// in mscorlib.dll so should not need to include extra references
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace MyUtility
{
    public static class Utility
    {
//------------------------------------------------------------------------CONSTANTS:

        public static int TAG_SPACE = 20;
        public static float EPSILON = 0.001f;

//--------------------------------------------------------------------------METHODS:

        /// <summary>
        /// Ensures all displays 0-numDisplaysToActivate are activated
        /// </summary>
        /// <param name="numDisplaysToActivate"></param>
        public static void ActivateDisplays( int numDisplaysToActivate )
        {            
            //#if ! UNITY_EDITOR
            //if( Display.displays.Length > 1 )
            //{
            //    for( int i = 1; i <= numDisplaysToActivate; i++ )
            //    {
            //        if( ! Display.displays[i].active )
            //        {
            //            Display.displays[i].Activate();
            //            ScreenPrinter.Instance.Print( i + 2, "Activate display " + i );
            //        }
            //    }
            //}
            //#endif
        }

        public static bool CanMove( this SphereCollider collider, Vector3 movement )
        {
            // We will cast a sphere with a radius half as big as the given collider.
            float radius = collider.radius * collider.transform.localScale.x * 0.5f;

            Vector3 origin = collider.transform.position +
                             movement.normalized * radius;

            Ray probeRay = new Ray( origin, movement );

            RaycastHit[] hits = Physics.SphereCastAll( probeRay, radius );
            foreach( RaycastHit hit in hits )
            {
                if( hit.collider != collider  &&  
                    hit.distance <= ( movement.magnitude ) )
                {
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Returns true if given collider is inside any other objects with colliders
        /// in the scene.
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="rayLength"></param>
        /// <returns></returns>
        public static bool ColliderInsideObject( Collider collider,
                                                 float rayLength = 1000 )
        {
            return ObjColliderIsInside( collider, rayLength ) != null;
        }

        public static Bounds Combine( this Bounds[] allBounds )
        {
            Bounds bounds = new Bounds( Vector3.zero, Vector3.zero );

            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;

            foreach( Bounds objBounds in allBounds )
            {
                maxX = Mathf.Max( maxX, objBounds.max.x );
                maxY = Mathf.Max( maxY, objBounds.max.y );
                maxZ = Mathf.Max( maxZ, objBounds.max.z );

                minX = Mathf.Min( minX, objBounds.min.x );
                minY = Mathf.Min( minY, objBounds.min.y );
                minZ = Mathf.Min( minZ, objBounds.min.z );
            }
            bounds.SetMinMax( new Vector3( minX, minY, minZ ),
                              new Vector3( maxX, maxY, maxZ ) );
            return bounds;
        }

        public static Rect CombineRects( Rect a, Rect b )
        {
            Rect enclosingRect = new Rect();
            enclosingRect.xMax = Mathf.Max( a.xMax, b.xMax );
            enclosingRect.xMin = Mathf.Min( a.xMin, b.xMin );
            enclosingRect.yMax = Mathf.Max( a.yMax, b.yMax );
            enclosingRect.yMin = Mathf.Min( a.yMin, b.yMin );
            return enclosingRect;
        }

        public static Vector3[] CornersAndCenter( this Bounds bounds )
        {
            Vector3[] points = new Vector3[9];

            Vector3 toCorner = bounds.extents;

            // forward, top right
            points[0] = bounds.center + toCorner;
            // backward, bottom left (if still facing forward)
            points[1] = bounds.center - toCorner;

            toCorner.x *= -1;
            points[2] = bounds.center + toCorner; // forward, top left
            points[3] = bounds.center - toCorner; // back, bottom right

            toCorner.z *= -1;
            points[4] = bounds.center + toCorner; // back, top right
            points[5] = bounds.center - toCorner; // forward, bottom right

            toCorner.x *= -1;
            points[6] = bounds.center + toCorner; // forward, bottom left
            points[7] = bounds.center - toCorner; // backward, bottom right

            points[8] = bounds.center;
            return points;
        }

        public static Vector3[] CornerPoints( this Bounds bounds )
        {
            Vector3[] points = new Vector3[8];

            Vector3 toCorner = bounds.extents;

            // forward, top right
            points[0] = bounds.center + toCorner;
            // backward, bottom left (if still facing forward)
            points[1] = bounds.center - toCorner;

            toCorner.x *= -1;
            points[2] = bounds.center + toCorner; // forward, top left
            points[3] = bounds.center - toCorner; // back, bottom right

            toCorner.z *= -1;
            points[4] = bounds.center + toCorner; // back, top right
            points[5] = bounds.center - toCorner; // forward, bottom right

            toCorner.x *= -1;
            points[6] = bounds.center + toCorner; // forward, bottom left
            points[7] = bounds.center - toCorner; // backward, bottom right

            return points;
        }

        public static void DrawRay( Vector3 pos, Vector3 dir )
        {
            Debug.DrawLine( pos, pos + dir * 100 );
        }

        public static void DrawRay( Vector3 pos, Vector3 dir, Color color )
        {
            Debug.DrawLine( pos, pos + dir * 100, color );
        }

        public static void DrawRay( Ray ray, float len = 100 )
        {
            Debug.DrawLine( ray.origin, ray.origin + ray.direction.normalized * len );

        }

        public static void DrawRay( Ray ray, Color color, float len = 100 )
        {
            Debug.DrawLine( ray.origin, ray.origin + ray.direction.normalized * len, color );
        }

        public static void EnableCollidersInChildren( this GameObject obj, bool enable )
        {
            if( obj == null )  return;            

            Collider thisRenderer = obj.GetComponent<Collider>();
            if( thisRenderer != null )
            {
                thisRenderer.enabled = enable;
            }
            Collider[] components = obj.GetComponentsInChildren<Collider>();
            foreach( Collider collider in components )
            {
                collider.enabled = enable;
            }
        }
        
        public static T EnsureComponent<T>( this GameObject obj ) where T : MonoBehaviour
        {
            T component = obj.GetComponent<T>();
            if( component == null )
            {
                return obj.AddComponent<T>();
            }
            return component;
        }

        public static Vector3 FindCenter( this GameObject[] gameObjects )
        {
            Vector3 sum = Vector3.zero;
            foreach( GameObject gameObject in gameObjects )
            {
                sum += gameObject.transform.position;
            }
            if( gameObjects.Length > 0 )
            {
                return sum / gameObjects.Length;
            }
            return default( Vector3 );
        }
        
        public static T FindComponent<T>( this GameObject obj )
        {
            T component = obj.GetComponentInChildren<T>();
            if( component != null )   return component;
            return obj.GetComponentInParent<T>();
        }

        public static Vector3 ForwardRelativeToRoot( this Transform transform )
        {
            return transform.root.InverseTransformDirection( transform.forward );
        }

        public static Rect FrustumAtDistance( this Camera camera, float distance )
        {
            float angle = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;
            float height = 2.0f * distance * Mathf.Tan( angle );

            Rect frustum = new Rect();
            frustum.width = height * camera.aspect;
            frustum.height = frustum.width / camera.aspect;
            frustum.center = camera.transform.position +
                             camera.transform.forward * distance;
            return frustum;
        }

        /// <summary>
        /// Enables or disables the renderers in this game object and all children
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="enable"></param>
        public static void EnableRenderersInChildren( this GameObject obj, bool enable )
        {
            if( obj == null )   return;            

            Renderer thisRenderer = obj.GetComponent<Renderer>();
            if( thisRenderer != null )
            {
                thisRenderer.enabled = enable;
            }
            Renderer[] components = obj.GetComponentsInChildren<Renderer>();
            foreach( Renderer renderer in components )
            {
                renderer.enabled = enable;
            }
        }

        /// <summary>
        /// Returns the bounds of given GameObject and its children's Renderers
        /// </summary>    
        public static Bounds GetBounds( this GameObject gameObject )
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            if( renderers.Length == 0 )
            {
                return GetBoundsFromColliders( gameObject );
            }
            Bounds[] allBounds = renderers.Select( x => x.bounds ).ToArray();
            return Combine( allBounds );
        }
        
        /// <summary>
        /// Returns the bounds of a given GameObject and it's children's Colliders
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static Bounds GetBoundsFromColliders( GameObject gameObject )
        {
            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            Bounds[] allBounds = colliders.Select( x => x.bounds ).ToArray();
            return Combine( allBounds );
        }
        
        /// <summary>
        /// Returns all values in enum T
        /// </summary>
        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues( typeof( T ) ).Cast<T>();
        }

        /// <summary>
        /// Returns the largest extent of bounding box of given gameObject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static float GetMaxExtent( this GameObject gameObject )
        {
            Bounds bounds = GetBounds( gameObject );
            float maxExtent = bounds.extents.x;
            maxExtent = Mathf.Max( maxExtent, bounds.extents.y );
            maxExtent = Mathf.Max( maxExtent, bounds.extents.z );
            return maxExtent;
        }

        /// <summary>
        /// Returns the smallest extent of bounding box of given gameObject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static float GetMinExtent( this GameObject gameObject )
        {
            Bounds bounds = GetBounds( gameObject );
            float minExtent = bounds.extents.x;
            minExtent = Mathf.Min( minExtent, bounds.extents.y );
            minExtent = Mathf.Min( minExtent, bounds.extents.z );
            return minExtent;
        }

        // Warning: Assumes you have the key saved
        public static bool GetPlayerPrefBool( string tag )
        {
            int prefValue = PlayerPrefs.GetInt( tag );
            return prefValue > 0;
        }

        /// <summary>
        /// Returns horizontal field of view in degrees
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static float HorizontalFOV( this Camera cam )
        {
            float radAngle = cam.fieldOfView * Mathf.Deg2Rad;
            double hFOV = 2 * Math.Atan( Mathf.Tan( radAngle / 2 ) * cam.aspect );
            return (float)( Mathf.Rad2Deg * hFOV );
        }

        /// <summary>
        /// Each collider will ignore collisions with each other collider
        /// </summary>
        /// <param name="colliders"></param>
        /// <param name="ignore"></param>
        public static void IgnoreCollisions( Collider[] colliders, bool ignore = true )
        {
            for( int i = 0; i < colliders.Length; i++ )
            {
                for( int j = i + 1; j < colliders.Length; j++ )
                {
                    Physics.IgnoreCollision( colliders[i], colliders[j], ignore );
                }
            }
        }

        /// <summary>
        /// A O(n) string to int parser 
        /// </summary>
        /// <param name="colliders"></param>   
        public static int IntParseFast(string value)
        {
        int result = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char letter = value[i];
            result = 10 * result + (letter - 48);
        }
        return result;
        }
        /// <summary>
        /// Returns true if given text is only letters (a-z or A-Z)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsAllLetters( this string text )
        {
            foreach( char character in text )
            {
                if( ! character.IsLetter() )
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if given character is a letter (a-z or A-Z)
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool IsLetter( this char character )
        {
            int ascii = (int)character;
            return ( ascii >= 65  &&  ascii <= 90 )  ||     // Is uppercase letter
                   ( ascii >= 97  &&  ascii <= 122 );       // Is lowercase letter
        }

        /// <summary>
        /// Returns the linear velocity of given Rigidbody
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <returns></returns>
        public static Vector3 LinearVelocity( this Rigidbody rigidbody )
        {
            float currentSpeed = rigidbody.velocity.magnitude;
            float linearSpeed = Vector3.Dot( rigidbody.velocity, 
                                             rigidbody.transform.forward );
            return rigidbody.transform.forward * linearSpeed;
        }

        /// <summary>
        /// Returns the GameObject of the object the collider is inside if the collider
        /// is inside an object
        /// in the scene.
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="rayLength"></param>
        /// <returns></returns>
        public static GameObject ObjColliderIsInside( Collider collider,
                                                      float rayLength = 1000 )
        {
            // Shoot two rays, one from way ahead and one from way behind.  We can
            // decide if we're in an object based on the order of collisions of these
            // two rays
            Vector3 colliderPos = collider.transform.position;
            Vector3 ahead = collider.transform.forward;

            Collider[] fromAhead = lastTwoHitsFromDirection( colliderPos, ahead, rayLength );
            if( fromAhead == null ) return null;
            Collider lastThingHitFromAhead = fromAhead[0];
            Collider secondToLastThingHitFromAhead = fromAhead[1];

            Collider[] fromBehind = lastTwoHitsFromDirection( colliderPos, -ahead, rayLength );
            if( fromBehind == null ) return null;
            Collider lastThingHitFromBehind = fromBehind[0];
            Collider secondToLastThingHitFromBehind = fromBehind[1];

            if( lastThingHitFromAhead != collider )
            {
                return lastThingHitFromAhead.gameObject;
            }
            if( lastThingHitFromBehind != collider )
            {
                return lastThingHitFromBehind.gameObject;
            }
            if( secondToLastThingHitFromAhead == secondToLastThingHitFromBehind )
            {
                return secondToLastThingHitFromAhead.gameObject;
            }
            return null;
        }

        public static Vector3 ParseVec3( string vector )
        {
            if( vector.StartsWith( "(" ) )
            {
                vector = vector.Split( '(' )[1].Split( ')' )[0];
            }
            string[] components = vector.Split( ',' );
            float x = float.Parse( components[0] );
            float y = float.Parse( components[1] );
            float z = float.Parse( components[2] );
            return new Vector3( x, y, z );
        }

        /// <summary>
        /// Plays the given AudioSource's clip at the source's position
        /// </summary>
        /// <param name="audioSource"></param>
        public static void PlayAtSource( AudioSource audioSource )
        {
            if( audioSource != null )
            {
                AudioSource.PlayClipAtPoint( audioSource.clip,
                                             audioSource.transform.position );
            }
        }

        /// <summary>
        /// Returns the screen space coordinate of the point this ray hits
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static Vector2 ToScreenSpace( this Camera camera, Ray ray )
        {
            RaycastHit hit;
            if( Physics.Raycast( ray, out hit ) )
            {
                Vector3 pixelCoordinate = camera.WorldToScreenPoint( hit.point );
                return new Vector2( pixelCoordinate.x / camera.pixelWidth,
                                    pixelCoordinate.y / camera.pixelHeight );
            }
            return Vector2.zero;
        }

        /// <summary>
        /// Prints given message with the caller's string used as a tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        public static void TPrint( this string tag, string message )
        {
            Print( tag, message );            
        }

        /// <summary>
        /// Removes and returns the last item added to given list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Pop<T>( this List<T> list )
        {
            if( list.Count == 0 )    return default( T );

            T item = list[list.Count - 1];
            list.RemoveAt( list.Count - 1 );

            return item;
        }

        /// <summary>
        /// Prints given message preceded by given tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        public static void Print( string tag, string message )
        {
            MonoBehaviour.print( tag + " --- " + message );
        }

        /// <summary>
        /// Prints error with given message and tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        public static void PrintError( string tag, string message )
        {
            Debug.LogError( tag + " --- " + message );
        }
        
        public static bool RandBool()
        {
            return UnityEngine.Random.Range( 0f, 1f ) >= 0.5;
        }

        // Generate a random point within the given Bounds
        public static Vector3 RandomPointInBounds( Bounds bounds )
        {
            return RandomVectorInRange( bounds.min, bounds.max );
        }


        public static Vector3 RandomVectorInRange( Vector3 min, Vector3 max )
        {
            return new Vector3( UnityEngine.Random.Range( min.x, max.x ),
                                UnityEngine.Random.Range( min.y, max.y ),
                                UnityEngine.Random.Range( min.z, max.z ) );
        }

        public static string RemoveWhitespace( this string str )
        {
            return string.Join( "", 
                                str.Split( default( string[] ), 
                                StringSplitOptions.RemoveEmptyEntries ) );
        }
        
        public static void SetPlayerPref( string tag, bool isTrue )
        {
            if( isTrue )
            {
                PlayerPrefs.SetInt( tag, 1 );
            }
            else
            {
                PlayerPrefs.SetInt( tag, 0 );
            }
        }

        /// <summary>
        /// Returns true if given numbers are both positive or are both negative
        /// </summary>
        /// <param name="num"></param>
        /// <param name="otherNum"></param>
        /// <returns></returns>
        public static bool SignAgrees( this float num, float otherNum )
        {
            return ( num >= 0  &&  otherNum >= 0 ) ||
                   ( num < 0   &&  otherNum < 0 );
        }

        public static string Truncate( this string str, int maxLength )
        {
            return str.Substring( 0, Mathf.Min( str.Length, maxLength ) );
        }

        /// <summary>
        /// Returns the given text with quotes around it
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string WithQuotes( this string text )
        {
            return "\"" + text + "\"";
        }


        /// <summary>
        /// Grabs the value of a [possible] Description attribute of an Enum value.
        /// Returns the default toString of the value if none exists.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>NVL( Description Attribute, default Enum.ToString() )</returns>
        public static string GetEnumDescription(Enum value)
        {
            // Get the name of the particular enum value
            FieldInfo info = value.GetType().GetField(value.ToString());

            // Get any description attributes it has
            DescriptionAttribute[] attributes = 
                (DescriptionAttribute[])info
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            // Return accordingly whether any attributes are found
            return (attributes != null && attributes.Length > 0) ?
                attributes[0].Description : value.ToString();
        }


//--------------------------------------------------------------------------HELPERS:

        /// <summary>
        /// Returns the last two things hit by ray.  
        /// Index 0 is last thing hit, Index 1 is second to last thing hit
        /// </summary>
        /// <param name="colliderPos"></param>
        /// <param name="rayDirection">Direction of </param>
        /// <param name="rayLength"></param>
        /// <returns></returns>
        private static Collider[] lastTwoHitsFromDirection( Vector3 colliderPos,
                                                            Vector3 rayDirection,
                                                            float rayLength )
    {
        Vector3 rayOrigin = colliderPos + rayDirection * rayLength;
        RaycastHit[] hits = Physics.RaycastAll( rayOrigin, -rayDirection, rayLength );

        if( hits.Length == 0 ) Debug.LogError( "that makes no sense" );
        if( hits.Length <= 1 ) return null;
        // Order the hits by distance so the last thing hit will be the collider
        hits = hits.OrderBy( h => h.distance ).ToArray();

        //Debug.DrawLine( rayOrigin, colliderPos, Color.cyan );

        Collider lastThingHit = hits[hits.Length - 1].collider;
        Collider secondToLastThingHit = hits[hits.Length - 2].collider;

        Collider[] hitColliders = new Collider[2];
        hitColliders[0] = lastThingHit;
        hitColliders[1] = secondToLastThingHit;

        return hitColliders;
    }
        
    }
}
