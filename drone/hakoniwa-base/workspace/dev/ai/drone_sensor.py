import numpy as np
from pyquaternion import Quaternion
from scipy.spatial.transform import Rotation as R
import math

def quaternion_to_euler_xyz(q):
    norm = math.sqrt(q['x']**2 + q['y']**2 + q['z']**2 + q['w']**2)
    if norm == 0.0:
        return [0.0, 0.0, 0.0]
    normalized_quaternion = [q['x'] / norm, q['y'] / norm, q['z'] / norm, q['w'] / norm]
    r = R.from_quat(normalized_quaternion)
    return r.as_euler('xyz', degrees=True)

