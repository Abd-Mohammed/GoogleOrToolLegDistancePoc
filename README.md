# Google Or Toole Leg Distance Poc

### System should generate routes that adhere to the determined maximum allowed distance between consecutive stops (Example: if the leg distance is 2 km, then the leg distance for all generated routes should not exceed these 2 km).


# Test Cases

## Test Case 1

**Distance Matrix:**

|     | 0  | 10 | 50 |
|-----|----|----|----|
| 0   | 0  | 10 | 50 |
| 10  | 10 | 0  | 150|
| 50  | 50 | 150| 0  |

**Vehicle Number:** 1
**Result:** Solution Not Found!

---

## Test Case 2

**Distance Matrix:**

|     | 0  | 10 | 50 |
|-----|----|----|----|
| 0   | 0  | 10 | 50 |
| 10  | 10 | 0  | 20 |
| 50  | 50 | 20 | 0  |

**Vehicle Number:** 1

**Result:**
- **Route:** 0 -> 1 -> 2 -> 0
- **Distance of the route:** 80m
- **Maximum distance of the routes:** 80m
- **Total distance of the routes:** 80m

---

## Test Case 3

**Distance Matrix:**

|     | 0  | 10 | 50 |
|-----|----|----|----|
| 0   | 0  | 10 | 50 |
| 10  | 10 | 0  | 150|
| 50  | 50 | 150| 0  |

**Vehicle Number:** 2

**Result:**

- **Route for Vehicle 0:** 0 -> 2 -> 0
  - **Distance of the route:** 100m
- **Route for Vehicle 1:** 0 -> 1 -> 0
  - **Distance of the route:** 20m
- **Maximum distance of the routes:** 100m
- **Total distance of the routes:** 200m
