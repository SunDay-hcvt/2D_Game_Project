using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed; //Tốc độ của nền bay
    public int startingPoint; //Vị trí bắt đầu của nền bay
    public Transform[] points; //Mảng các điểm mà nền bay sẽ đi qua

    public int i; //Phần tử của mảng
    void Start()
    {
        //Đặt vị trí nền bay về vị trí ban đầu
        transform.position = points[startingPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        //Kiểm tra khoảng cách giữa nền bay và điểm i
        //Nếu khoảng cách bằng 0 thì tăng i lên
        if (Vector2.Distance(transform.position, points[i].position) == 0)
        {
            i++;
            if (i == points.Length) //Nếu i đạt phần tử cuối thì reset về phần tử đầu
            {
                i = 0;
            }
        }
        //Chuyển động nền bay tới vị trí điểm i
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }

    //Khi nhân vật nhảy lên nền bay, nền bay sẽ trở thành phần tử cha của nhân vật
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.transform.SetParent(transform);
    }
    //Khi nhân vật rời ra khỏi nền bay thì ngược lại
    private void OnCollisionExit2D(Collision2D collision)
    {
        collision.transform.SetParent(null);
    }
}

// using UnityEngine;

// public class MovingPlatform : MonoBehaviour 
// {
//     public float speed; // Tốc độ di chuyển
//     public float smoothTime; // Thời gian làm mượt chuyển động
//     public int startingPoint; // Điểm bắt đầu
//     public Transform[] points; // Mảng các điểm

//     private int currentIndex; // Index điểm hiện tại
//     private Vector2 currentVelocity; // Vận tốc hiện tại cho SmoothDamp    
//     void Start()
//     {
//         currentIndex = startingPoint;
//         transform.position = points[currentIndex].position;
//     }

//     void Update()
//     {
//         Vector2 nextPoint = points[(currentIndex + 1) % points.Length].position;
        
//         // Sử dụng SmoothDamp để làm mượt chuyển động
//         transform.position = Vector2.SmoothDamp(
//             transform.position, 
//             nextPoint, 
//             ref currentVelocity, 
//             smoothTime,
//             speed
//         );

//         // Kiểm tra khoảng cách đến điểm tiếp theo
//         float distanceToNext = Vector2.Distance(transform.position, nextPoint);
//         if (distanceToNext < 0.1f)
//         {
//             currentIndex = (currentIndex + 1) % points.Length;
//         }
//     }

//     // Thêm nhân vật vào làm con của platform
//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         collision.transform.SetParent(transform);
//     }

//     // Gỡ bỏ nhân vật khỏi platform
//     private void OnCollisionExit2D(Collision2D collision)
//     {
//         collision.transform.SetParent(null);
//     }
// }