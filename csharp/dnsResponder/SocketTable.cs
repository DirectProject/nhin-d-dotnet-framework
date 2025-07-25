﻿/* 
 Copyright (c) 2010, Direct Project
 All rights reserved.

 Authors:
    Umesh Madan     umeshma@microsoft.com
 
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of The Direct Project (directproject.org) nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Health.Direct.DnsResponder
{
    public class SocketTable
    {
        Dictionary<long, Socket> m_socketTable;
        long m_nextID = 0;
        
        public SocketTable()
        {
            m_socketTable = new Dictionary<long,Socket>();
        }
        
        public int Count
        {
            get
            {
                lock(m_socketTable)
                {
                    return m_socketTable.Count;
                }
            }
        }
        
        public Socket[] GetSockets()
        {
            lock(m_socketTable)
            {
                return m_socketTable.Values.ToArray();
            }
        }
        
        public Socket this[long socketID]
        {
            get
            {
                lock(m_socketTable)
                {
                    Socket socket;
                    if (!m_socketTable.TryGetValue(socketID, out socket))
                    {
                        socket = null;
                    }
                    
                    return socket;
                }
            }
        }
        
        public long Add(Socket socket)
        {            
            if (socket == null)
            {
                throw new ArgumentNullException();
            }
            
            lock(m_socketTable)
            {   
                ++m_nextID;
                m_socketTable.Add(m_nextID, socket);
                return m_nextID;
            }
        }
        
        public bool Remove(long socketID)
        {
            try
            {
                lock(m_socketTable)
                {
                    m_socketTable.Remove(socketID);
                    return true;
                }
            }
            catch
            {
            }
            
            return false;
        }
        
        public void Shutdown(long socketID)
        {
            this.Shutdown(socketID, -1);
        }
        
        public void Shutdown(long socketID, int timeout)
        {
            Socket socket = null;
            lock(m_socketTable)
            {
                socket = this[socketID];
                if (socket != null)
                {
                    this.Remove(socketID);
                }
            }
            
            if (socket != null)
            {
                socket.SafeShutdownAndClose(SocketShutdown.Both, timeout);
            }
        }
        
        public void Shutdown()
        {
            lock(m_socketTable)
            {
                foreach(Socket socket in m_socketTable.Values)
                {
                    socket.SafeClose();
                }
            }
        }
        
        public void Clear()
        {
            lock(m_socketTable)
            {
                this.Shutdown();
                m_socketTable.Clear();
            }
        }
    }
}