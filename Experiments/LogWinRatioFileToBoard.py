import tensorflow as tf
import numpy as np

def log_win_rates(file_path, log_dir='win_rates'):
    writer = tf.summary.create_file_writer(log_dir)
    
    with open(file_path, 'r') as file:
        data = file.readlines()

    with writer.as_default():
        for line in data:
            if line.startswith("#"):  
                continue
            step, win_rate = map(float, line.strip().split())
            tf.summary.scalar('Defender WinRate', win_rate, step=int(step))
            writer.flush()

if __name__ == '__main__':
    data_file = 'parsed_wr.txt'
    log_win_rates(data_file)